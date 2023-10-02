using Stride.Core.Mathematics;

namespace WorldBuilding.Helpers
{
    public class NeighborsHelper
    {
        public static readonly Vector3[] Neighbor =
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