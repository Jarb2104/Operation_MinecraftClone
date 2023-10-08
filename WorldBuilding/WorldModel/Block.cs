﻿using Stride.Core.Mathematics;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;
using WorldBuilding.Mathematics;

namespace WorldBuilding.WorldModel
{
    public class Block
    {
        public Vector3SByte BlockCoords { get; }
        public Biome Biome { get; set; }
        public BlockType Type
        {
            get
            {
                return GetBlockType();
            }
        }
        public bool IsTerrain { get; set; }
        public float BlockScale { get; }
        public Chunk ParentChunk { get; }

        public Block(Chunk parentChunk, sbyte x, sbyte y, sbyte z, bool isTerrain, float blockScale)
        {
            BlockCoords = new Vector3SByte(x, y, z);
            IsTerrain = isTerrain;
            BlockScale = blockScale;
            ParentChunk = parentChunk;
        }

        public List<Vector3> GetVertices()
        {
            return CubeHelpers.Vertices.Select(v => v * BlockScale + 
            new Vector3(
                BlockCoords.X * BlockScale + ParentChunk.ChunkCoords.X * ParentChunk.ChunkWidthSize * BlockScale,
                BlockCoords.Y * BlockScale + ParentChunk.ChunkCoords.Y * ParentChunk.ChunkHeightSize * BlockScale, 
                BlockCoords.Z * BlockScale + ParentChunk.ChunkCoords.Z * ParentChunk.ChunkLenghtSize * BlockScale)).ToList();
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
                        <= 70 and > 55 => BlockType.Snow,
                        <= 55 and > 40 => BlockType.Rock,
                        <= 40 and > 20 => BlockType.Grass,
                        <= 20 and > 0 => BlockType.Dirt,
                        _ => BlockType.Grass,
                    };
                    return result;
                default: return result;
            }
        }
    }
}
