using Stride.Core.Mathematics;
using WorldBuilding.Mathematics;

namespace WorldBuilding.Helpers
{
    public static class CubeHelpers
    {
        public static readonly Vector3[] Normals =
        {
            new Vector3 (-1, -1, -1),
            new Vector3 ( 1, -1, -1),
            new Vector3 (-1,  1, -1),
            new Vector3 ( 1,  1, -1),
            new Vector3 (-1, -1,  1),
            new Vector3 ( 1, -1,  1),
            new Vector3 (-1,  1,  1),
            new Vector3 ( 1,  1,  1),
        };

        public static readonly Vector3[] Vertices =
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1)
        };

        public static readonly Vector3SByte[] BlockNeighbors =
        {
            new Vector3SByte (0, 0, -1),
            new Vector3SByte (0, 0, 1),
            new Vector3SByte (-1, 0, 0),
            new Vector3SByte (1, 0, 0),
            new Vector3SByte (0, -1, 0),
            new Vector3SByte (0, 1, 0),
        };

        public static readonly Vector3[] ChunkNeighbors =
        {
            new Vector3 (0, 0, -1),
            new Vector3 (0, 0, 1),
            new Vector3 (-1, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (0, -1, 0),
            new Vector3 (0, 1, 0),
        };
    }
}