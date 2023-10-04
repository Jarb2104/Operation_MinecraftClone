﻿using Stride.Core.Diagnostics;
using WorldBuilding.WorldModel;

namespace WorldBuilding
{
    public class WorldGenerator
    {
        public static World GenerateWorld(int seed, short worldSize, short worldHeight, short chunkSize, Logger Log)
        {
            Log.Debug($"{DateTime.Now.ToLongTimeString()} | Starting with world creation");
            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | Seed: {seed} - WorldSize: {worldSize} - WorldHeight: {worldHeight} - ChunkSize: {chunkSize} ");
            World newWorld = new(seed, worldSize, worldHeight, chunkSize);
            newWorld.CreateWorldChunks(Log);
            return newWorld;
        }
    }
}
