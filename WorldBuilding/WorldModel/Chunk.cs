using Stride.Core.Mathematics;
using WorldBuilding.Enums;
using WorldBuilding.WorldDefinitions;
using WorldBuilding.Helpers;
using WorldBuilding.CustomErrors;
using WorldBuilding.Mathematics;
using Stride.Core.Diagnostics;

namespace WorldBuilding.WorldModel
{
    public class Chunk
    {
        public Vector3 ChunkCoords { get; }
        public sbyte ChunkSize { get; }
        public sbyte ChunkHeightSize { get; }
        public bool IsVisible { get; set; }
        public World ParentWorld { get; }
        public Dictionary<Vector3SByte, Block> ChunkBlocks = null!;

        public Chunk(World parentWorld, Vector3 chunkCoords, sbyte chunkSize, sbyte chunkHeightSize)
        {
            ChunkCoords = chunkCoords;
            ParentWorld = parentWorld;
            ChunkSize = chunkSize;
            ChunkHeightSize = chunkHeightSize;
            IsVisible = false;
            ChunkBlocks = new Dictionary<Vector3SByte, Block>();
        }

        public void GenerateTerrain(float blockScale, short worldHeight, FastNoiseLite fastNoise)
        {
            //ulong NoiseOffset = ulong.MaxValue / 2;
            Dictionary<Vector3SByte, Block> chunkBlocks = new();
            Random terrainRandomer = new();
            decimal[,] terrainNoise = new decimal[ChunkSize, ChunkSize];
            decimal xCoordinate;
            decimal zCoordinate;

            for (sbyte x = 0; x < ChunkSize; x++)
            {
                for (sbyte z = 0; z < ChunkSize; z++)
                {
                    xCoordinate = (decimal)(ChunkCoords.X * ChunkSize + x);
                    zCoordinate = (decimal)(ChunkCoords.Z * ChunkSize + z);
                    terrainNoise[x, z] = fastNoise.GetNoise(xCoordinate * WorldVariables.MountainScale, zCoordinate * WorldVariables.MountainScale);
                    terrainNoise[x, z] = Interpolate(terrainNoise[x, z], worldHeight);
                }
            }

            decimal biomeNoise = 1;
            for (sbyte x = 0; x < ChunkSize; x++)
            {
                for (sbyte z = 0; z < ChunkSize; z++)
                {
                    for (sbyte y = ChunkHeightSize; y >= 0; y--)
                    {
                        GenerateBlock(new Vector3SByte(x, y, z), blockScale, terrainNoise[x, z], worldHeight, biomeNoise, terrainRandomer, chunkBlocks);
                    }
                }
            }

            ChunkBlocks = chunkBlocks;
        }

        public bool CheckVisibility()
        {
            Span<KeyValuePair<Vector3SByte, Block>> spanBlocks = ChunkBlocks.ToArray();
            Block? neighborBlock;

            foreach (KeyValuePair<Vector3SByte, Block> block in spanBlocks)
            {
                foreach (FaceSides face in Enum.GetValues(typeof(FaceSides)))
                {
                    neighborBlock = GetNeighbor(block.Key, face);
                    neighborBlock ??= GetNeighborFromNeighborChunk(block.Key, face);

                    if (neighborBlock == null || neighborBlock.BlockType == BlockTypes.Air)
                    {
                        block.Value.IsVisible = true;
                        IsVisible = true;
                        break;
                    }
                }
            }

            return IsVisible;
        }

        private static decimal Interpolate(decimal value, short worldHeight)
        {
            return (value + 1) * (worldHeight / 2);
        }

        private void GenerateBlock(Vector3SByte blockCoords, float blockScale, decimal terrainHeight, short worldHeight, decimal biomeNoise, Random terrainRandomer, Dictionary<Vector3SByte, Block> chunkBlocks)
        {
            Vector3SByte neighborCoords;
            int cubeHeight = blockCoords.Y + (int)ChunkCoords.Y * ChunkHeightSize;
            int cubeOffSet = (int)terrainHeight - cubeHeight;
            double mountainCutOffComparer = Math.Round((double)terrainHeight / worldHeight * 100);
            BlockTypes currentBlockType = cubeHeight <= terrainHeight ? BlockTypes.Grass : BlockTypes.Air;

            Block newBlock = new(this, blockCoords, blockScale)
            {
                Biome = (sbyte)Math.Floor(biomeNoise) switch
                {
                    <= WorldConstants.ForestThreshold => Biomes.Forest,
                    > WorldConstants.ForestThreshold and <= WorldConstants.DesertThreshold => Biomes.Desert,
                    > WorldConstants.DesertThreshold and <= WorldConstants.JungleThreshold => Biomes.Jungle,
                    > WorldConstants.JungleThreshold and <= WorldConstants.TundraThreshold => Biomes.Tundra,
                    > WorldConstants.TundraThreshold and <= WorldConstants.IcyThreshold => Biomes.Icy,
                    > WorldConstants.IcyThreshold and <= WorldConstants.SwampThreshold => Biomes.Swamp,
                    _ => Biomes.Plains
                },
            };

            //This is a placeholder need to pull out the BlockType calculation, include different types of biomes, and change how mountain blocktypes are placed depending on the height of the mountain
            if (currentBlockType != BlockTypes.Air)
            {
                if (cubeOffSet == 1)
                {
                    currentBlockType = BlockTypes.Grass;
                }
                else if (cubeOffSet >= 2 && cubeOffSet < 7)
                {
                    currentBlockType = BlockTypes.Dirt;
                }
                else if (cubeOffSet >= 7 && cubeOffSet < 34)
                {
                    currentBlockType = BlockTypes.Rock;
                }
                else if (cubeOffSet >= 34)
                {
                    currentBlockType = BlockTypes.Corrupt;
                }

                if (mountainCutOffComparer > 25)
                {
                    currentBlockType = BlockTypes.Rock;

                    if (terrainHeight > 210)
                    {
                        currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 210 && terrainHeight > 205)
                    {
                        if (terrainRandomer.Next(10) < 9)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 205 && terrainHeight > 200)
                    {
                        if (terrainRandomer.Next(10) < 8)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 200 && terrainHeight > 195)
                    {
                        if (terrainRandomer.Next(10) < 7)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 195 && terrainHeight > 190)
                    {
                        if (terrainRandomer.Next(10) < 6)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 190 && terrainHeight > 185)
                    {
                        if (terrainRandomer.Next(10) < 5)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 185 && terrainHeight > 175)
                    {
                        if (terrainRandomer.Next(10) < 4)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 175 && terrainHeight > 165)
                    {
                        if (terrainRandomer.Next(10) < 3)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (terrainHeight <= 165 && terrainHeight > 150)
                    {
                        if (terrainRandomer.Next(10) < 2)
                            currentBlockType = BlockTypes.Snow;
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 180 && terrainHeight > 155)
                    {
                        if (terrainRandomer.Next(10) < 3)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 155 && terrainHeight > 140)
                    {
                        if (terrainRandomer.Next(10) < 4)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 140 && terrainHeight > 130)
                    {
                        if (terrainRandomer.Next(10) < 5)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 130 && terrainHeight > 120)
                    {
                        if (terrainRandomer.Next(10) < 6)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 120 && terrainHeight > 115)
                    {
                        if (terrainRandomer.Next(10) < 7)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 115 && terrainHeight > 110)
                    {
                        if (terrainRandomer.Next(10) < 8)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 0 && terrainHeight <= 110)
                    {
                        if (terrainRandomer.Next(10) < 9)
                        {
                            currentBlockType = BlockTypes.Grass;
                        }
                    }

                    if (cubeOffSet == 1 && terrainHeight <= 150)
                    {
                        neighborCoords = blockCoords + CubeHelpers.BlockNeighbors[(sbyte)FaceSides.Top];
                        if (ChunkBlocks.ContainsKey(neighborCoords))
                        {
                            if (ChunkBlocks[neighborCoords].BlockType == BlockTypes.Grass)
                            {
                                currentBlockType = BlockTypes.Dirt;
                            }
                        }
                    }
                }
            }

            newBlock.BlockType = currentBlockType;
            chunkBlocks.Add(blockCoords, newBlock);
        }

        public Block? GetNeighbor(Vector3SByte blockCoords, FaceSides face)
        {
            Vector3SByte neighBorCoords = blockCoords + CubeHelpers.BlockNeighbors[(sbyte)face];
            ChunkBlocks.TryGetValue(neighBorCoords, out Block? neighbor);
            return neighbor;
        }

        public Block? GetNeighborFromNeighborChunk(Vector3SByte blockCoords, FaceSides face)
        {
            Chunk? NeighborChunk = ParentWorld.GetChunkNeighbor(ChunkBlocks[blockCoords].ParentChunk.ChunkCoords, face);
            if (NeighborChunk == null || NeighborChunk.ChunkBlocks.Count == 0)
            {
                return null;
            }

            Vector3SByte neighborBlock = new(blockCoords.X, blockCoords.Y, blockCoords.Z);
            switch (face)
            {
                case FaceSides.Front:
                    neighborBlock.Z = (sbyte)(ChunkSize - 1);
                    break;
                case FaceSides.Back:
                    neighborBlock.Z = 0;
                    break;
                case FaceSides.Left:
                    neighborBlock.X = (sbyte)(ChunkSize - 1);
                    break;
                case FaceSides.Right:
                    neighborBlock.X = 0;
                    break;
                case FaceSides.Bottom:
                    neighborBlock.Y = (sbyte)(ChunkHeightSize - 1);
                    break;
                case FaceSides.Top:
                    neighborBlock.Y = 0;
                    break;
            }

            return NeighborChunk.ChunkBlocks[neighborBlock];
        }
    }
}
