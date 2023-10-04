using SimplexNoise;
using WorldBuilding.Enums;
using WorldBuilding.WorldDefinitions;
using Stride.Core.Mathematics;
using WorldBuilding.Helpers;
using WorldBuilding.CustomErrors;
using Stride.Core.Diagnostics;

namespace WorldBuilding.WorldModel
{
    public class Chunk
    {
        public Vector3 ChunkCoords { get; }
        public short ChunkSize { get; }
        public World ParentWorld { get; }
        public readonly Dictionary<Vector3, Block> chunkBlocks = null!;

        public Chunk(World parentWorld, short x, short y, short z, short chunkSize)
        {
            ChunkCoords = new Vector3(x, y, z);
            ParentWorld = parentWorld;
            ChunkSize = chunkSize;
            chunkBlocks = new Dictionary<Vector3, Block>();
        }

        public async Task GenerateTerrain(int seed, float blockScale, Logger Log)
        {
            Noise.Seed = seed;
            // Combine noise results

            await Task.Run(() =>
            {
                Log.Warning($"{DateTime.Now.ToLongTimeString()} | Generating chunk {ChunkCoords}");
                for (short i = 0; i < ChunkSize; i++)
                {
                    for (short j = 0; j < ChunkSize; j++)
                    {
                        for (short k = 0; k < ChunkSize; k++)
                        {
                            float mountainNoise = Noise.CalcPixel2D(i + (int)ChunkCoords.X * ChunkSize, k + (int)ChunkCoords.Z * ChunkSize, WorldVariables.MountainScale) * WorldVariables.MountainAmplitude / 100;
                            //float detailNoise = Noise.CalcPixel2D(i + (int)ChunkCoords.X * Width, k + (int)ChunkCoords.Z * Length, WorldVariables.DetailScale) * WorldVariables.DetailAmplitude / 1000;
                            float biomeNoise = Noise.CalcPixel2D(i + (int)ChunkCoords.X * ChunkSize, k + (int)ChunkCoords.Z * ChunkSize, WorldVariables.BiomeScale) * WorldVariables.BiomeAmplitude / 100;

                            float terrainHeight = mountainNoise;// + detailNoise;
                            bool isTerrain = j + ChunkCoords.Y * ChunkSize < QuantizeToInterval(terrainHeight);
                            chunkBlocks.Add(new Vector3(i, j, k), (double)Math.Floor(biomeNoise) switch
                            {
                                <= WorldConstants.ForestThreshold => new Block(this, i, j, k, Biome.Forest, isTerrain, blockScale),
                                > WorldConstants.ForestThreshold and <= WorldConstants.DesertThreshold => new Block(this, i, j, k, Biome.Desert, isTerrain, blockScale),
                                > WorldConstants.DesertThreshold and <= WorldConstants.JungleThreshold => new Block(this, i, j, k, Biome.Jungle, isTerrain, blockScale),
                                > WorldConstants.JungleThreshold and <= WorldConstants.TundraThreshold => new Block(this, i, j, k, Biome.Tundra, isTerrain, blockScale),
                                > WorldConstants.TundraThreshold and <= WorldConstants.IcyThreshold => new Block(this, i, j, k, Biome.Icy, isTerrain, blockScale),
                                > WorldConstants.IcyThreshold and <= WorldConstants.SwampThreshold => new Block(this, i, j, k, Biome.Swamp, isTerrain, blockScale),
                                _ => new Block(this, i, j, k, Biome.Plains, isTerrain, blockScale),
                            });
                        }
                    }
                }
                Log.Info($"{DateTime.Now.ToLongTimeString()} | Chunk {ChunkCoords} finished generating");
            });
        }

        public Block? GetNeighbor(Vector3 blockCoords, FaceSide face)
        {
            if (chunkBlocks.ContainsKey(blockCoords))
            {
                Vector3 neighBorCoords = blockCoords + CubeHelpers.Neighbors[(short)face];
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

        public Block? GetNeighborFromNeighborChunk(Vector3 blockCoords, FaceSide face)
        {
            Chunk? NeighborChunk = ParentWorld.GetChunkNeighbor(chunkBlocks[blockCoords].ParentChunk.ChunkCoords, face);
            if (NeighborChunk == null)
            {
                return null;
            }

            Vector3 neighborBlock = new(blockCoords.X, blockCoords.Y, blockCoords.Z);
            switch (face)
            {
                case FaceSide.Front:
                    neighborBlock.Z = ChunkSize-1;
                    break;
                case FaceSide.Back:
                    neighborBlock.Z = 0;
                    break;
                case FaceSide.Left:
                    neighborBlock.X = ChunkSize-1;
                    break;
                case FaceSide.Right:
                    neighborBlock.X = 0;
                    break;
                case FaceSide.Bottom:
                    neighborBlock.Y = ChunkSize-1;
                    break;
                case FaceSide.Top:
                    neighborBlock.Y = 0;
                    break;
            }

            return chunkBlocks[neighborBlock];
        }

        private static short QuantizeToInterval(float val)
        {
            // Quantize to the nearest interval
            return (short)Math.Round(val, 0);
        }
    }
}
