using Stride.Core.Diagnostics;
using WorldBuilding.WorldModel;

namespace WorldBuilding
{
    public class WorldGenerator
    {
        private readonly Logger Log = null!;
        public WorldGenerator(Logger logger)
        {
            Log = logger;
        }

        public async Task<World> GenerateWorld(int seed, short worldSize, short worldHeight, short chunkSize)
        {
            World newWorld = new(seed, worldSize, worldHeight, chunkSize, Log);
            Log.Info("Start generating chunks");
            await newWorld.GenerateWorldChunks();
            return newWorld;
        }
    }
}
