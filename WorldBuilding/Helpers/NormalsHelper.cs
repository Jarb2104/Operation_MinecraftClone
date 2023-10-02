using Stride.Core.Mathematics;

namespace WorldBuilding.Helpers
{
    public class NormalsHelper
    {
        public static readonly Vector3[] Normal =
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
    }
}