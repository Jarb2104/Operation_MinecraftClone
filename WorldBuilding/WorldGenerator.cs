using Stride.Core.Diagnostics;
using WorldBuilding.WorldModel;

namespace WorldBuilding
{
    public class WorldGenerator
    {
        public static World GenerateWorld(int seed, short worldHeight, sbyte chunkWidthSize, sbyte chunkLengthSize, Logger Log)
        {
            Log.Debug($"{DateTime.Now.ToLongTimeString()} | Starting with world creation");
            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | Seed: {seed} - WorldHeight: {worldHeight} - ChunkWidthSize: {chunkWidthSize} - ChunkLengthSize: {chunkLengthSize} ");
            World newWorld = new(seed, worldHeight, chunkWidthSize, chunkLengthSize);
            return newWorld;
        }
    }
}
