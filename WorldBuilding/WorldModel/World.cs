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
        public sbyte ChunkWidthSize { get; }
        public sbyte ChunkLengthSize { get; }
        public sbyte ChunkHeightSize { get; }
        public readonly Dictionary<Vector3, Chunk> worldChunks = null!;

        public World(int worldSeed, short worldSize, short worldHeight, sbyte chunkWidthSize, sbyte chunkLengthSize)
        {
            WorldSeed = worldSeed;
            WorldSize = worldSize;
            Height = (short)Math.Ceiling((decimal)worldHeight / worldSize);
            ChunkWidthSize = chunkWidthSize;
            ChunkLengthSize = chunkLengthSize;
            ChunkHeightSize = (sbyte)Math.Ceiling((decimal)worldHeight / Height);
            worldChunks = new Dictionary<Vector3, Chunk>();
        }

        public void CreateWorldChunks(Logger Log)
        {
            for (short i = 0; i < WorldSize; i++)
            {
                for (short j = 0; j < Height; j++)
                {
                    for (short k = 0; k < WorldSize; k++)
                    {
                        Vector3 v = new(i, j, k);
                        worldChunks.Add(v, new Chunk(this, i, j, k, ChunkWidthSize, ChunkLengthSize, ChunkHeightSize));
                        Log.Info($"{DateTime.Now.ToLongTimeString()} | Chunk {v} created and ready to be generated.");
                    }
                }
            }

            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | All world chunks generated");
        }

        public async Task GenerateChunkTerrain(Vector3 chunkCoords, float blockScale, Logger Log)
        {
            Log.Warning($"{DateTime.Now.ToLongTimeString()} | Chunk {chunkCoords} terrain generation started");
            await worldChunks[chunkCoords].GenerateTerrain(WorldSeed, blockScale, Log);
            Log.Info($"{DateTime.Now.ToLongTimeString()} | Chunk {chunkCoords} terrain generation finished");
        }

        public Chunk? GetChunkNeighbor(Vector3 chunkCoords, FaceSide face)
        {
            if (worldChunks.ContainsKey(chunkCoords))
            {
                Vector3 neighBorCoords = new(
                    chunkCoords.X + CubeHelpers.BlockNeighbors[(short)face].X,
                    chunkCoords.Y + CubeHelpers.BlockNeighbors[(short)face].Y,
                    chunkCoords.Z + CubeHelpers.BlockNeighbors[(short)face].Z);
                if (worldChunks.ContainsKey(neighBorCoords))
                {
                    return (Chunk?)worldChunks[neighBorCoords];
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
    }
}
