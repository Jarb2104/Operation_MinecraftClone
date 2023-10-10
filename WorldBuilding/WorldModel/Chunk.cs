using Stride.Core.Mathematics;
using Stride.Core.Diagnostics;
using WorldBuilding.Enums;
using WorldBuilding.WorldDefinitions;
using WorldBuilding.Helpers;
using WorldBuilding.CustomErrors;
using WorldBuilding.Mathematics;

namespace WorldBuilding.WorldModel
{
    public class Chunk
    {
        public Vector3 ChunkCoords { get; }
        public sbyte ChunkWidthSize { get; }
        public sbyte ChunkLenghtSize { get; }
        public sbyte ChunkHeightSize { get; }
        public World ParentWorld { get; }
        public Dictionary<Vector3SByte, Block> chunkBlocks = null!;

        public Chunk(World parentWorld, int x, int y, int z, sbyte chunkWidthSize, sbyte chunkLengthSize, sbyte chunkHeightSize)
        {
            ChunkCoords = new Vector3(x, y, z);
            ParentWorld = parentWorld;
            ChunkWidthSize = chunkWidthSize;
            ChunkLenghtSize = chunkLengthSize;
            ChunkHeightSize = chunkHeightSize;
            chunkBlocks = new Dictionary<Vector3SByte, Block>();
        }

        public async Task GenerateTerrain(float blockScale, int worldHeight, Noise simplexNoise, Logger Log)
        {
            Log.Warning($"{DateTime.Now.ToLongTimeString()} | Generating chunk {ChunkCoords}");
            chunkBlocks = await Task.Run(() => GenerateChunkBlocks(blockScale, (int)ChunkCoords.X, (int)ChunkCoords.Y, (int)ChunkCoords.Z, ChunkWidthSize, ChunkLenghtSize, ChunkHeightSize, worldHeight, simplexNoise, Log));
        }

        public Dictionary<Vector3SByte, Block> GenerateChunkBlocks(float blockScale, int X, int Y, int Z, sbyte chunkWidth, sbyte chunkLength, sbyte chunkHeight, int worldHeight, Noise simplexNoise, Logger Log)
        {
            Dictionary<Vector3SByte, Block> newChunkBlocks = new();
            Random ran = new();
            for (sbyte i = 0; i < chunkWidth; i++)
            {
                for (sbyte k = 0; k < chunkLength; k++)
                {
                    float mountainNoise = simplexNoise.CalcPixel2D((ulong)(i + X * chunkWidth), (ulong)(k + Z * chunkLength), WorldVariables.MountainScale) * WorldVariables.MountainAmplitude / 70;
                    float detailNoise = simplexNoise.CalcPixel2D((ulong)(i + X * chunkWidth), (ulong)(k + Z * chunkLength), WorldVariables.DetailScale) * WorldVariables.DetailAmplitude / 100;
                    float biomeNoise = simplexNoise.CalcPixel2D((ulong)(i + X * chunkWidth), (ulong)(k + Z * chunkLength), WorldVariables.BiomeScale) * WorldVariables.BiomeAmplitude;
                    int terrainHeight = (int)Math.Round(mountainNoise + detailNoise, 0);
                    decimal mountainCutOffComparer = Math.Round((decimal)terrainHeight / worldHeight * 100);
                    bool Wasgrass = false;

                    for (sbyte j = chunkHeight; j > -1; j--)
                    {
                        int cubeHeight = j + Y * chunkHeight;
                        bool isTerrain = cubeHeight < terrainHeight;
                        int cubeOffSet = terrainHeight - cubeHeight;
                        //This is a placeholder need to pull out the BlockType calculation, include different types of biomes, and change how mountain blocktypes are placed depending on the height of the mountain
                        BlockTypes currentBlockType = BlockTypes.Air;

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
                                if (ran.Next(10) < 9)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 205 && terrainHeight > 200)
                            {
                                if (ran.Next(10) < 8)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 200 && terrainHeight > 195)
                            {
                                if (ran.Next(10) < 7)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 195 && terrainHeight > 190)
                            {
                                if (ran.Next(10) < 6)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 190 && terrainHeight > 185)
                            {
                                if (ran.Next(10) < 5)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 185 && terrainHeight > 175)
                            {
                                if (ran.Next(10) < 4)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 175 && terrainHeight > 165)
                            {
                                if (ran.Next(10) < 3)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (terrainHeight <= 165 && terrainHeight > 150)
                            {
                                if (ran.Next(10) < 2)
                                    currentBlockType = BlockTypes.Snow;
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 180 && terrainHeight > 155)
                            {
                                if (ran.Next(10) < 3)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 155 && terrainHeight > 140)
                            {
                                if (ran.Next(10) < 4)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 140 && terrainHeight > 130)
                            {
                                if (ran.Next(10) < 5)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 130 && terrainHeight > 120)
                            {
                                if (ran.Next(10) < 6)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 120 && terrainHeight > 115)
                            {
                                if (ran.Next(10) < 7)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 115 && terrainHeight > 110)
                            {
                                if (ran.Next(10) < 8)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 1 && terrainHeight <= 110)
                            {
                                if (ran.Next(10) < 9)
                                {
                                    currentBlockType = BlockTypes.Grass;
                                    Wasgrass = true;
                                }
                            }

                            if (cubeOffSet == 2 && terrainHeight <= 150)
                            {
                                if (Wasgrass)
                                {
                                    currentBlockType = BlockTypes.Dirt;
                                    Wasgrass = false;
                                }
                            }
                        }

                        Block newBlock = new(this, i, j, k, isTerrain, blockScale)
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
                            BlockType = currentBlockType,
                        };
                        newChunkBlocks.Add(new Vector3SByte(i, j, k), newBlock);
                    }
                }
            }
            Log.Info($"{DateTime.Now.ToLongTimeString()} | Chunk {ChunkCoords} finished generating");
            return newChunkBlocks;
        }

        public Block? GetNeighbor(Vector3SByte blockCoords, FaceSide face)
        {
            if (chunkBlocks.ContainsKey(blockCoords))
            {
                Vector3SByte neighBorCoords = blockCoords + CubeHelpers.BlockNeighbors[(short)face];
                if (chunkBlocks.ContainsKey(neighBorCoords))
                {
                    return (Block?)chunkBlocks[neighBorCoords];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new BlockOutOfRangeException("Block coordinates are out of range.");
            }
        }

        public Block? GetNeighborFromNeighborChunk(Vector3SByte blockCoords, FaceSide face)
        {
            Chunk? NeighborChunk = ParentWorld.GetChunkNeighbor(chunkBlocks[blockCoords].ParentChunk.ChunkCoords, face);
            if (NeighborChunk == null)
            {
                return null;
            }

            Vector3SByte neighborBlock = new(blockCoords.X, blockCoords.Y, blockCoords.Z);
            switch (face)
            {
                case FaceSide.Front:
                    neighborBlock.Z = (sbyte)(ChunkLenghtSize - 1);
                    break;
                case FaceSide.Back:
                    neighborBlock.Z = 0;
                    break;
                case FaceSide.Left:
                    neighborBlock.X = (sbyte)(ChunkWidthSize - 1);
                    break;
                case FaceSide.Right:
                    neighborBlock.X = 0;
                    break;
                case FaceSide.Bottom:
                    neighborBlock.Y = (sbyte)(ChunkHeightSize - 1);
                    break;
                case FaceSide.Top:
                    neighborBlock.Y = 0;
                    break;
            }

            return NeighborChunk.chunkBlocks[neighborBlock];
        }
    }
}
