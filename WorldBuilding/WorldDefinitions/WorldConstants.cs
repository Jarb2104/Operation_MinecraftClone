using WorldBuilding.Enums;

namespace WorldBuilding.WorldDefinitions
{
    public static class WorldConstants
    {
        public const Biome DefaultBiome = Biome.Plains;

        // Define biome thresholds
        public const float ForestThreshold = 0f;
        public const float DesertThreshold = 2f;
        public const float JungleThreshold = 4f;
        public const float TundraThreshold = 6f;
        public const float IcyThreshold = 8f;
        public const float SwampThreshold = 10f;
        public const float PlainsThreshold = 12f;
    }
}
