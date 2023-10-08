using Stride.Core.Diagnostics;
using WorldBuilding.WorldModel;

namespace WorldBuilding
{
    public class WorldGenerator
    {
        public static World GenerateWorld(int seed, short worldSize, short worldHeight, sbyte chunkWidthSize, sbyte chunkLengthSize, Logger Log)
        {
            Log.Debug($"{DateTime.Now.ToLongTimeString()} | Starting with world creation");
            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | Seed: {seed} - WorldSize: {worldSize} - WorldHeight: {worldHeight} - ChunkWidthSize: {chunkWidthSize} - ChunkLengthSize: {chunkLengthSize} ");
            World newWorld = new(seed, worldSize, worldHeight, chunkWidthSize, chunkLengthSize);
            newWorld.CreateWorldChunks(Log);
            return newWorld;
        }
    }
}
