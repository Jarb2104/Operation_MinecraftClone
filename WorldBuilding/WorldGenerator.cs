using Stride.Core.Diagnostics;
using WorldBuilding.WorldModel;

namespace WorldBuilding
{
    public class WorldGenerator
    {
        public static World GenerateWorld(int seed, short worldHeight, sbyte chunkSize, Logger Log)
        {
            Log.Debug($"{DateTime.Now.ToLongTimeString()} | Starting with world creation");
            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | Seed: {seed} - WorldHeight: {worldHeight} - ChunkSize: {chunkSize}");
            World newWorld = new(seed, worldHeight, chunkSize);
            return newWorld;
        }
    }
}
