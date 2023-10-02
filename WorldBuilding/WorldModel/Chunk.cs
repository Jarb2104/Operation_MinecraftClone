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
        public short Width { get; }
        public short Length { get; }
        public short Height { get; }
        public World ParentWorld { get; }
        public readonly Dictionary<Vector3, Block> chunkBlocks = null!;

        public Chunk(World parentWorld, short x, short y, short z, short chunkSize)
        {
            ChunkCoords = new Vector3(x, y, z);
            ParentWorld = parentWorld;
            Width = chunkSize;
            Length = chunkSize;
            Height = chunkSize;
            chunkBlocks = new Dictionary<Vector3, Block>();
        }

        public async Task GenerateTerrain(int seed, Logger Log)
        {
            Noise.Seed = seed;
            // Combine noise results

            await Task.Run(() =>
            {
                Log.Warning($"{DateTime.Now.ToLongTimeString} | Generating chunk {ChunkCoords}");
                for (short i = 0; i < Length; i++)
                {
                    for (short j = 0; j < Width; j++)
                    {
                        for (short k = 0; k < Height; k++)
                        {
                            float mountainNoise = Noise.CalcPixel2D(i + (int)ChunkCoords.X * Width, k + (int)ChunkCoords.Z * Length, WorldVariables.MountainScale) * WorldVariables.MountainAmplitude / 1000;
                            float detailNoise = Noise.CalcPixel2D(i + (int)ChunkCoords.X * Width, k + (int)ChunkCoords.Z * Length, WorldVariables.DetailScale) * WorldVariables.DetailAmplitude / 1000;
                            float biomeNoise = Noise.CalcPixel2D(i + (int)ChunkCoords.X * Width, k + (int)ChunkCoords.Z * Length, WorldVariables.BiomeScale) * WorldVariables.BiomeAmplitude / 100;

                            float terrainHeight = mountainNoise + detailNoise;

                            chunkBlocks.Add(new Vector3(i, j, k), (double)Math.Floor(biomeNoise) switch
                            {
                                <= WorldConstants.ForestThreshold => new Block(this, i, j, k, Biome.Forest, j < QuantizeToInterval(terrainHeight)),
                                > WorldConstants.ForestThreshold and <= WorldConstants.DesertThreshold => new Block(this, i, j, k, Biome.Desert, j < QuantizeToInterval(terrainHeight)),
                                > WorldConstants.DesertThreshold and <= WorldConstants.JungleThreshold => new Block(this, i, j, k, Biome.Jungle, j < QuantizeToInterval(terrainHeight)),
                                > WorldConstants.JungleThreshold and <= WorldConstants.TundraThreshold => new Block(this, i, j, k, Biome.Tundra, j < QuantizeToInterval(terrainHeight)),
                                > WorldConstants.TundraThreshold and <= WorldConstants.IcyThreshold => new Block(this, i, j, k, Biome.Icy, j < QuantizeToInterval(terrainHeight)),
                                > WorldConstants.IcyThreshold and <= WorldConstants.SwampThreshold => new Block(this, i, j, k, Biome.Swamp, j < QuantizeToInterval(terrainHeight)),
                                _ => new Block(this, i, j, k, Biome.Plains, j < QuantizeToInterval(terrainHeight)),
                            });
                        }
                    }
                }
                Log.Info($"{DateTime.Now.ToLongTimeString} | Chunk {ChunkCoords} finished generating");
            });
        }

        public Block? GetNeighbor(Vector3 blockCoords, FaceSide face)
        {
            if (chunkBlocks.ContainsKey(blockCoords))
            {
                Vector3 neighBorCoords = blockCoords + NeighborsHelper.Neighbor[(short)face];
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

        private static short QuantizeToInterval(float val)
        {
            // Quantize to the nearest interval
            return (short)Math.Round(val, 0);
        }
    }
}
