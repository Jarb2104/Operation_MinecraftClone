using Stride.Core.Mathematics;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;
using WorldBuilding.Mathematics;

namespace WorldBuilding.WorldModel
{
    public class Block
    {
        public Vector3SByte BlockCoords { get; }
        public Biomes Biome { get; set; }
        public BlockTypes BlockType { get; set; }
        public bool IsVisible { get; set; }
        public float BlockScale { get; }
        public Chunk ParentChunk { get; }

        public Block(Chunk parentChunk, Vector3SByte blockCoords, float blockScale)
        {
            ParentChunk = parentChunk;
            BlockCoords = blockCoords;
            BlockScale = blockScale;
        }

        public List<Vector3> GetVertices()
        {
            return CubeHelpers.Vertices.Select(v => v * BlockScale + 
            new Vector3(
                BlockCoords.X * BlockScale + ParentChunk.ChunkCoords.X * ParentChunk.ChunkSize * BlockScale,
                BlockCoords.Y * BlockScale + ParentChunk.ChunkCoords.Y * ParentChunk.ChunkHeightSize * BlockScale, 
                BlockCoords.Z * BlockScale + ParentChunk.ChunkCoords.Z * ParentChunk.ChunkSize * BlockScale)).ToList();
        }

        public List<Vector3> GetFaceVertices(FaceSides side)
        {
            List<Vector3> resultVertices = null!;
            List<Vector3> vertices = GetVertices();

            switch (side)
            {
                case FaceSides.Front:
                    resultVertices = new List<Vector3> { vertices[0], vertices[1], vertices[2], vertices[3] };
                    break;
                case FaceSides.Back:
                    resultVertices = new List<Vector3> { vertices[4], vertices[5], vertices[6], vertices[7] };
                    break;
                case FaceSides.Left:
                    resultVertices = new List<Vector3> { vertices[0], vertices[2], vertices[4], vertices[6] };
                    break;
                case FaceSides.Right:
                    resultVertices = new List<Vector3> { vertices[1], vertices[3], vertices[5], vertices[7] };
                    break;
                case FaceSides.Bottom:
                    resultVertices = new List<Vector3> { vertices[0], vertices[1], vertices[4], vertices[5] };
                    break;
                case FaceSides.Top:
                    resultVertices = new List<Vector3> { vertices[2], vertices[3], vertices[6], vertices[7] };
                    break;
            }

            return resultVertices;
        }
    }
}
