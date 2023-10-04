using Stride.Core.Mathematics;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;

namespace WorldBuilding.WorldModel
{
    public class Block
    {
        public Vector3 BlockCoords { get; }
        public Biome Biome { get; set; }
        public bool IsTerrain { get; set; }
        public float BlockScale { get; }
        public Chunk ParentChunk { get; }

        public Block(Chunk parentChunk, float x, float y, float z, Biome biome, bool isTerrain, float blockScale)
        {
            BlockCoords = new Vector3(x, y, z);
            Biome = biome;
            IsTerrain = isTerrain;
            BlockScale = blockScale;
            ParentChunk = parentChunk;
        }

        public List<Vector3> GetVertices()
        {
            return CubeHelpers.Vertices.Select(v => v * BlockScale + BlockCoords * BlockScale + ParentChunk.ChunkCoords * ParentChunk.ChunkSize * BlockScale).ToList();
        }

        public List<Vector3> GetFaceVertices(FaceSide side)
        {
            List<Vector3> resultVertices = null!;
            List<Vector3> vertices = GetVertices();

            switch (side)
            {
                case FaceSide.Front:
                    resultVertices = new List<Vector3> { vertices[0], vertices[1], vertices[2], vertices[3] };
                    break;
                case FaceSide.Back:
                    resultVertices = new List<Vector3> { vertices[4], vertices[5], vertices[6], vertices[7] };
                    break;
                case FaceSide.Left:
                    resultVertices = new List<Vector3> { vertices[0], vertices[2], vertices[4], vertices[6] };
                    break;
                case FaceSide.Right:
                    resultVertices = new List<Vector3> { vertices[1], vertices[3], vertices[5], vertices[7] };
                    break;
                case FaceSide.Bottom:
                    resultVertices = new List<Vector3> { vertices[0], vertices[1], vertices[4], vertices[5] };
                    break;
                case FaceSide.Top:
                    resultVertices = new List<Vector3> { vertices[2], vertices[3], vertices[6], vertices[7] };
                    break;
            }

            return resultVertices;
        }

        public BlockType GetBlockType()
        {
            List<Vector3> vertices = GetVertices();
            BlockType result = BlockType.Grass;
            switch (Biome)
            {
                case Biome.Plains:
                    result = (vertices[0].Y) switch
                    {
                        >= 40 => BlockType.Snow,
                        >= 30 and < 40 => BlockType.Rock,
                        >= 20 and < 30 => BlockType.Grass,
                        >= 10 and < 20 => BlockType.Dirt,
                        _ => BlockType.Grass,
                    };
                    return result;
                default: return result;
            }
        }
    }
}
