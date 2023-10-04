using Stride.Core.Mathematics;

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

        public static readonly List<Vector3> Vertices = new()
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f)
        };

        public static readonly Vector3[] Neighbors =
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