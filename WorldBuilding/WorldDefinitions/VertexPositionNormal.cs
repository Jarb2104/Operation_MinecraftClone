// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Runtime.InteropServices;
using Stride.Core.Mathematics;
using Stride.Graphics;

namespace WorldBuilding.WorldDefinitions
{
    /// <summary>
    /// Describes a custom vertex format structure that contains position and color information. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexPositionNormal : IEquatable<VertexPositionNormal>, IVertex
    {
        /// <summary>
        /// Initializes a new <see cref="VertexPositionTexture"/> instance.
        /// </summary>
        /// <param name="position">The position of this vertex.</param>
        /// <param name="normal">Normal of the vertex for ilumination.</param>
        public VertexPositionNormal(Vector3 position, Vector3 normal)
            : this()
        {
            Position = position;
            Normal = normal;
        }

        /// <summary>
        /// XYZ position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// UV texture coordinates.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Defines structure byte size.
        /// </summary>
        public static readonly int Size = Vector3.SizeInBytes*2;

        /// <summary>
        /// The vertex layout of this struct.
        /// </summary>
        public static readonly VertexDeclaration Layout = new(VertexElement.Position<Vector3>(), VertexElement.Normal<Vector3>());

        public bool Equals(VertexPositionNormal other)
        {
            return Position.Equals(other.Position) && Normal.Equals(other.Normal);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            return obj is VertexPositionNormal vertex && Equals(vertex);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Position, Normal);
        }

        public readonly VertexDeclaration GetLayout()
        {
            return Layout;
        }

        public readonly void FlipWinding(){}

        public static bool operator ==(VertexPositionNormal left, VertexPositionNormal right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionNormal left, VertexPositionNormal right)
        {
            return !left.Equals(right);
        }

        public override readonly string ToString()
        {
            return string.Format($"Position: {Position}, Normal: {Normal}");
        }
    }
}
