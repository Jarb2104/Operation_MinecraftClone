using Silk.NET.Core;
using Stride.Core.Mathematics;
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using WorldBuilding.Enums;

namespace WorldBuilding.WorldModel
{
    public class Block
    {
        public Vector3 BlockCoords { get; }
        public Biome Biome { get; set; }
        public bool IsTerrain { get; set; }
        public Chunk ParentChunk { get; }

        public readonly List<Vector3> vertices = new()
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

        public Block(Chunk parentChunk, short x, short y, short z, Biome biome, bool isTerrain)
        {
            ParentChunk = parentChunk;
            BlockCoords = new Vector3(x, y, z);
            Biome = biome;
            IsTerrain = isTerrain;

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += BlockCoords;
            }
        }
        public void TranslateBlock(Vector3 offSet)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = vertices[i] + offSet;
            }
        }

        public void ScaleBlock(Vector3 scale)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = vertices[i] * scale;
            }
        }

        public List<Vector3> GetFaceVertices(FaceSide side)
        {
            List<Vector3> resultVertices = null!;
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
