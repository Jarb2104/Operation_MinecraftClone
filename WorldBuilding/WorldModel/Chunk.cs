using SimplexNoise;
using WorldBuilding.Enums;
using WorldBuilding.WorldDefinitions;
using Stride.Core.Mathematics;
using WorldBuilding.Helpers;
using WorldBuilding.CustomErrors;
using Stride.Core.Diagnostics;
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

        public Chunk(World parentWorld, short x, short y, short z, sbyte chunkWidthSize, sbyte chunkLengthSize, sbyte chunkHeightSize)
        {
            ChunkCoords = new Vector3(x, y, z);
            ParentWorld = parentWorld;
            ChunkWidthSize = chunkWidthSize;
            ChunkLenghtSize = chunkLengthSize;
            ChunkHeightSize = chunkHeightSize;
            chunkBlocks = new Dictionary<Vector3SByte, Block>();
        }

        public async Task GenerateTerrain(int seed, float blockScale, Logger Log)
        {
            Log.Warning($"{DateTime.Now.ToLongTimeString()} | Generating chunk {ChunkCoords}");
            chunkBlocks = await Task.Run(() => GenerateChunkBlocks(seed, blockScale, (int)ChunkCoords.X, (int)ChunkCoords.Y, (int)ChunkCoords.Z, ChunkWidthSize, ChunkHeightSize, ChunkLenghtSize, Log));
        }

        public Dictionary<Vector3SByte, Block> GenerateChunkBlocks(int seed, float blockScale, int X, int Y, int Z, sbyte chunkWidth, sbyte chunkHeight, sbyte chunkLength, Logger Log)
        {
            Noise.Seed = seed;
            Dictionary<Vector3SByte, Block> newChunkBlocks = new();
            for (sbyte i = 0; i < chunkWidth; i++)
            {
                for (sbyte k = 0; k < chunkLength; k++)
                {
                    for (sbyte j = (sbyte)(chunkHeight - 1); j > -1; j--)
                    {
                        float mountainNoise = Noise.CalcPixel2D(i + X * chunkWidth, k + Z * chunkLength, WorldVariables.MountainScale) * WorldVariables.MountainAmplitude / 100;
                        //float detailNoise = Noise.CalcPixel2D(i + X * chunkWidth, k + Z * chunkLength, WorldVariables.DetailScale) * WorldVariables.DetailAmplitude / 1000;
                        float biomeNoise = Noise.CalcPixel2D(i + X * chunkWidth, k + Z * chunkLength, WorldVariables.BiomeScale) * WorldVariables.BiomeAmplitude / 100;
                        float terrainHeight = mountainNoise;
                        bool isTerrain = j + Y * chunkHeight < (short)Math.Round(terrainHeight, 0);

                        Block newBlock = new(this, i, j, k, isTerrain, blockScale)
                        {
                            Biome = (double)Math.Floor(biomeNoise) switch
                            {
                                <= WorldConstants.ForestThreshold => Biome.Forest,
                                > WorldConstants.ForestThreshold and <= WorldConstants.DesertThreshold => Biome.Desert,
                                > WorldConstants.DesertThreshold and <= WorldConstants.JungleThreshold => Biome.Jungle,
                                > WorldConstants.JungleThreshold and <= WorldConstants.TundraThreshold => Biome.Tundra,
                                > WorldConstants.TundraThreshold and <= WorldConstants.IcyThreshold => Biome.Icy,
                                > WorldConstants.IcyThreshold and <= WorldConstants.SwampThreshold => Biome.Swamp,
                                _ => Biome.Plains
                            },
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
