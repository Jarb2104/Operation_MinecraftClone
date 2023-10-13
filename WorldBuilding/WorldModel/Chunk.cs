using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using WorldBuilding.Enums;
using WorldBuilding.WorldDefinitions;
using WorldBuilding.Helpers;
using WorldBuilding.CustomErrors;
using WorldBuilding.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using System.Runtime.InteropServices;

namespace WorldBuilding.WorldModel
{
    public class Chunk
    {
        public Vector3 ChunkCoords { get; }
        public sbyte ChunkWidthSize { get; }
        public sbyte ChunkLenghtSize { get; }
        public sbyte ChunkHeightSize { get; }
        public World ParentWorld { get; }
        public Dictionary<Vector3SByte, Block> ChunkBlocks = null!;

        public Chunk(World parentWorld, int x, int y, int z, sbyte chunkWidthSize, sbyte chunkLengthSize, sbyte chunkHeightSize)
        {
            ChunkCoords = new Vector3(x, y, z);
            ParentWorld = parentWorld;
            ChunkWidthSize = chunkWidthSize;
            ChunkLenghtSize = chunkLengthSize;
            ChunkHeightSize = chunkHeightSize;
            ChunkBlocks = new Dictionary<Vector3SByte, Block>();
        }

        public async Task GenerateTerrain(float blockScale, short worldHeight, Noise simplexNoise)
        {
            ChunkBlocks = await Task.Run(() => {
                Dictionary<Vector3SByte, Block> chunkBlocks = new();
                Random terrainRandomer = new();
                MatrixBuilder<float> matrixBuilder = Matrix<float>.Build;
                Matrix<float> terrainNoise = matrixBuilder.DenseOfArray(simplexNoise.Calc2DFromOffSet((ulong)ChunkCoords.X * (ulong)ChunkWidthSize, (ulong)ChunkWidthSize, (ulong)ChunkCoords.Z * (ulong)ChunkLenghtSize, (ulong)ChunkLenghtSize, WorldVariables.MountainScale));
                terrainNoise *= WorldVariables.MountainAmplitude;
                terrainNoise /= WorldVariables.MountainReducer;
                Matrix<float> detailNoise = matrixBuilder.DenseOfArray(simplexNoise.Calc2DFromOffSet((ulong)ChunkCoords.X * (ulong)ChunkWidthSize, (ulong)ChunkWidthSize, (ulong)ChunkCoords.Z * (ulong)ChunkLenghtSize, (ulong)ChunkLenghtSize, WorldVariables.DetailScale));
                detailNoise *= WorldVariables.DetailAmplitude;
                detailNoise /= WorldVariables.DetailReducer;
                Matrix<float> detailNoise2 = matrixBuilder.DenseOfArray(simplexNoise.Calc2DFromOffSet((ulong)ChunkCoords.X * (ulong)ChunkWidthSize, (ulong)ChunkWidthSize, (ulong)ChunkCoords.Z * (ulong)ChunkLenghtSize, (ulong)ChunkLenghtSize, WorldVariables.DetailScale2));
                detailNoise2 *= WorldVariables.DetailAmplitude2;
                detailNoise2 /= WorldVariables.DetailReducer2;
                Matrix<float> biomeNoise = matrixBuilder.DenseOfArray(simplexNoise.Calc2DFromOffSet((ulong)ChunkCoords.X * (ulong)ChunkWidthSize, (ulong)ChunkWidthSize, (ulong)ChunkCoords.Z * (ulong)ChunkLenghtSize, (ulong)ChunkLenghtSize, WorldVariables.BiomeScale));
                terrainNoise += detailNoise + detailNoise2;

                for (sbyte x = 0; x < ChunkWidthSize; x++)
                {
                    for (sbyte z = 0; z < ChunkLenghtSize; z++)
                    {
                        for (sbyte y = ChunkHeightSize; y >= 0; y--)
                        {
                            GenerateBlock(new Vector3SByte(x, y, z), blockScale, terrainNoise, worldHeight, biomeNoise, terrainRandomer, chunkBlocks);
                        }
                    }
                }

                return chunkBlocks;
            });
        }

        private void GenerateBlock(Vector3SByte blockCoords, float blockScale, Matrix<float> terrainHeight, short worldHeight, Matrix<float> biomeNoise, Random terrainRandomer, Dictionary<Vector3SByte, Block> chunkBlocks)
        {
            Vector3SByte neighborCoords;
            int cubeHeight = blockCoords.Y + (int)ChunkCoords.Y * ChunkHeightSize;
            bool isTerrain = cubeHeight < terrainHeight[blockCoords.X, blockCoords.Z];
            int cubeOffSet = (int)terrainHeight[blockCoords.X, blockCoords.Z] - cubeHeight;
            decimal mountainCutOffComparer = Math.Round((decimal)terrainHeight[blockCoords.X, blockCoords.Z] / worldHeight * 100);

            Block newBlock = new(this, blockCoords, isTerrain, blockScale)
            {
                Biome = (sbyte)Math.Floor(biomeNoise[blockCoords.X, blockCoords.Z]) switch
                {
                    <= WorldConstants.ForestThreshold => Biomes.Forest,
                    > WorldConstants.ForestThreshold and <= WorldConstants.DesertThreshold => Biomes.Desert,
                    > WorldConstants.DesertThreshold and <= WorldConstants.JungleThreshold => Biomes.Jungle,
                    > WorldConstants.JungleThreshold and <= WorldConstants.TundraThreshold => Biomes.Tundra,
                    > WorldConstants.TundraThreshold and <= WorldConstants.IcyThreshold => Biomes.Icy,
                    > WorldConstants.IcyThreshold and <= WorldConstants.SwampThreshold => Biomes.Swamp,
                    _ => Biomes.Plains
                },
                BlockType = BlockTypes.Grass,
            };

            //This is a placeholder need to pull out the BlockType calculation, include different types of biomes, and change how mountain blocktypes are placed depending on the height of the mountain
            BlockTypes currentBlockType = BlockTypes.Grass;
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

                if (terrainHeight[blockCoords.X, blockCoords.Z] > 210)
                {
                    currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 210 && terrainHeight[blockCoords.X, blockCoords.Z] > 205)
                {
                    if (terrainRandomer.Next(10) < 9)
                        currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 205 && terrainHeight[blockCoords.X, blockCoords.Z] > 200)
                {
                    if (terrainRandomer.Next(10) < 8)
                        currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 200 && terrainHeight[blockCoords.X, blockCoords.Z] > 195)
                {
                    if (terrainRandomer.Next(10) < 7)
                        currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 195 && terrainHeight[blockCoords.X, blockCoords.Z] > 190)
                {
                    if (terrainRandomer.Next(10) < 6)
                        currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 190 && terrainHeight[blockCoords.X, blockCoords.Z] > 185)
                {
                    if (terrainRandomer.Next(10) < 5)
                        currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 185 && terrainHeight[blockCoords.X, blockCoords.Z] > 175)
                {
                    if (terrainRandomer.Next(10) < 4)
                        currentBlockType = BlockTypes.Snow;
                }

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 175 && terrainHeight[blockCoords.X, blockCoords.Z] > 165)
                {
                    if (terrainRandomer.Next(10) < 3)
                        currentBlockType = BlockTypes.Snow;
                }   

                if (terrainHeight[blockCoords.X, blockCoords.Z] <= 165 && terrainHeight[blockCoords.X, blockCoords.Z] > 150)
                {
                    if (terrainRandomer.Next(10) < 2)
                        currentBlockType = BlockTypes.Snow;
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 180 && terrainHeight[blockCoords.X, blockCoords.Z] > 155)
                {
                    if (terrainRandomer.Next(10) < 3)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 155 && terrainHeight[blockCoords.X, blockCoords.Z] > 140)
                {
                    if (terrainRandomer.Next(10) < 4)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 140 && terrainHeight[blockCoords.X, blockCoords.Z] > 130)
                {
                    if (terrainRandomer.Next(10) < 5)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 130 && terrainHeight[blockCoords.X, blockCoords.Z] > 120)
                {
                    if (terrainRandomer.Next(10) < 6)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 120 && terrainHeight[blockCoords.X, blockCoords.Z] > 115)
                {
                    if (terrainRandomer.Next(10) < 7)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 115 && terrainHeight[blockCoords.X, blockCoords.Z] > 110)
                {
                    if (terrainRandomer.Next(10) < 8)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 0 && terrainHeight[blockCoords.X, blockCoords.Z] <= 110)
                {
                    if (terrainRandomer.Next(10) < 9)
                    {
                        currentBlockType = BlockTypes.Grass;
                    }
                }

                if (cubeOffSet == 1 && terrainHeight[blockCoords.X, blockCoords.Z] <= 150)
                {
                    neighborCoords = blockCoords + CubeHelpers.BlockNeighbors[(byte)FaceSides.Top];
                    if (ChunkBlocks.ContainsKey(neighborCoords))
                    {
                        if (ChunkBlocks[neighborCoords].BlockType == BlockTypes.Grass)
                        {
                            currentBlockType = BlockTypes.Dirt;
                        }
                    }
                }
            }

            newBlock.BlockType = currentBlockType;
            chunkBlocks.Add(blockCoords, newBlock);
        }

        public Block? GetNeighbor(Vector3SByte blockCoords, FaceSides face)
        {
            if (ChunkBlocks.ContainsKey(blockCoords))
            {
                Vector3SByte neighBorCoords = blockCoords + CubeHelpers.BlockNeighbors[(byte)face];
                if (ChunkBlocks.ContainsKey(neighBorCoords))
                {
                    return (Block?)ChunkBlocks[neighBorCoords];
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

        public Block? GetNeighborFromNeighborChunk(Vector3SByte blockCoords, FaceSides face)
        {
            Chunk? NeighborChunk = ParentWorld.GetChunkNeighbor(ChunkBlocks[blockCoords].ParentChunk.ChunkCoords, face);
            if (NeighborChunk == null)
            {
                return null;
            }

            Vector3SByte neighborBlock = new(blockCoords.X, blockCoords.Y, blockCoords.Z);
            switch (face)
            {
                case FaceSides.Front:
                    neighborBlock.Z = (sbyte)(ChunkLenghtSize - 1);
                    break;
                case FaceSides.Back:
                    neighborBlock.Z = 0;
                    break;
                case FaceSides.Left:
                    neighborBlock.X = (sbyte)(ChunkWidthSize - 1);
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
