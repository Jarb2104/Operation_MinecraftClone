using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
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
        public sbyte ChunkWidthSize { get; }
        public sbyte ChunkLengthSize { get; }
        public sbyte ChunkHeightSize { get; }
        public readonly Dictionary<Vector3, Chunk> worldChunks = null!;

        public World(int worldSeed, short worldHeight, sbyte chunkWidthSize, sbyte chunkLengthSize)
        {
            WorldSeed = worldSeed;
            WorldHeight = worldHeight;
            ChunkWidthSize = chunkWidthSize;
            ChunkLengthSize = chunkLengthSize;
            ChunkHeightSize = (sbyte)Math.Ceiling((decimal)worldHeight / WorldConstants.WorldHeightInChunks);
            worldChunks = new Dictionary<Vector3, Chunk>();
        }

        public void CreateWorldChunks(Logger Log, Vector2 initPosition, Vector2 endPosition)
        {
            int chunksInitPositionX = (int)Math.Ceiling(initPosition.X / ChunkWidthSize);
            int chunksEndPositionX = (int)Math.Ceiling(endPosition.X / ChunkWidthSize);
            int chunksInitPositionY = (int)Math.Ceiling(initPosition.Y / ChunkWidthSize);
            int chunksEndPositionY = (int)Math.Ceiling(endPosition.Y / ChunkWidthSize);

            for (int i = chunksInitPositionX; i < chunksEndPositionX; i++)
            {
                for (int j = chunksInitPositionY; j < chunksEndPositionY; j++)
                {
                    for (int k = 0; k < WorldConstants.WorldHeightInChunks; k++)
                    {
                        Vector3 v = new(i, j, k);
                        worldChunks.Add(v, new Chunk(this, i, j, k, ChunkWidthSize, ChunkLengthSize, ChunkHeightSize));
                    }
                }
            }

            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | All world chunks generated");
        }

        public Chunk? GetChunkNeighbor(Vector3 chunkCoords, FaceSides face)
        {
            if (worldChunks.ContainsKey(chunkCoords))
            {
                Vector3 neighBorCoords = new(
                    chunkCoords.X + CubeHelpers.BlockNeighbors[(byte)face].X,
                    chunkCoords.Y + CubeHelpers.BlockNeighbors[(byte)face].Y,
                    chunkCoords.Z + CubeHelpers.BlockNeighbors[(byte)face].Z);
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
