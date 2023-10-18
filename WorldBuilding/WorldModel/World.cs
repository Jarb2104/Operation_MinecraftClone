using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using System.Collections.Concurrent;
using WorldBuilding.CustomErrors;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;
using WorldBuilding.Mathematics;
using WorldBuilding.WorldDefinitions;

namespace WorldBuilding.WorldModel
{
    public class World
    {
        public int WorldSeed { get; }
        public int WorldHeight { get; }
        public sbyte ChunkSize { get; }
        public sbyte ChunkHeightSize { get; }
        public readonly Dictionary<Vector3, Chunk> worldChunks = null!;

        public World(int worldSeed, short worldHeight, sbyte chunkSize)
        {
            WorldSeed = worldSeed;
            WorldHeight = worldHeight;
            ChunkSize = chunkSize;
            ChunkHeightSize = (sbyte)Math.Ceiling((decimal)worldHeight / WorldConstants.WorldHeightInChunks);
            worldChunks = new Dictionary<Vector3, Chunk>();
        }

        public Chunk? GetChunkNeighbor(Vector3 chunkCoords, FaceSides face)
        {
            Vector3 neighBorCoords = new(
                chunkCoords.X + CubeHelpers.BlockNeighbors[(byte)face].X,
                chunkCoords.Y + CubeHelpers.BlockNeighbors[(byte)face].Y,
                chunkCoords.Z + CubeHelpers.BlockNeighbors[(byte)face].Z);
            worldChunks.TryGetValue(neighBorCoords, out Chunk? neighbor);
            return neighbor;
        }
    }
}
