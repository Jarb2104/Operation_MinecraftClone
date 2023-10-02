using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using WorldBuilding.CustomErrors;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;

namespace WorldBuilding.WorldModel
{
    public class World
    {
        public int WorldSeed { get; }
        public short WorldSize { get; }
        public short Height { get; }
        public short ChunkSize { get; }
        public readonly Dictionary<Vector3, Chunk> worldChunks = null!;

        public World(int worldSeed, short worldSize, short worldHeight, short chunkSize)
        {
            WorldSeed = worldSeed;
            WorldSize = worldSize;
            Height = worldHeight;
            ChunkSize = chunkSize;
            worldChunks = new Dictionary<Vector3, Chunk>();
        }

        public async Task GenerateWorldChunks(Logger Log)
        {
            for (short i = 0; i < WorldSize; i++)
            {
                for (short j = 0; j < Height; j++)
                {
                    for (short k = 0; k < WorldSize; k++)
                    {
                        Vector3 v = new(i, j, k);
                        worldChunks.Add(v, new Chunk(this, i, j, k, ChunkSize));
                        Log.Info($"{DateTime.Now.ToLongTimeString} | Chunk {v} created and ready to be generated.");
                    }
                }
            }
            
            await Task.WhenAll(worldChunks.Select(ck => ck.Value.GenerateTerrain(WorldSeed, Log)));
            Log.Verbose($"{DateTime.Now.ToLongTimeString} | All world chunks generated");
        }

        public Chunk? GetChunkNeighbor(Vector3 chunkCoords, FaceSide face)
        {
            if (worldChunks.ContainsKey(chunkCoords))
            {
                Vector3 neighborCoords = chunkCoords + NeighborsHelper.Neighbor[(short)face];
                if (worldChunks.ContainsKey(neighborCoords))
                {
                    return (Chunk?)worldChunks[neighborCoords];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new ChunkOutOfRangeException("Chunk coordinates are out of range.");
            }
        }
    }
}
