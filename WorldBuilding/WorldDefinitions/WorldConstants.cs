using WorldBuilding.Enums;

namespace WorldBuilding.WorldDefinitions
{
    public static class WorldConstants
    {
        public const Biomes DefaultBiome = Biomes.Plains;

        // Define biome thresholds
        public const sbyte ForestThreshold = 0;
        public const sbyte DesertThreshold = 2;
        public const sbyte JungleThreshold = 4;
        public const sbyte TundraThreshold = 6;
        public const sbyte IcyThreshold = 8;
        public const sbyte SwampThreshold = 10;
        public const sbyte PlainsThreshold = 12;
    }
}
