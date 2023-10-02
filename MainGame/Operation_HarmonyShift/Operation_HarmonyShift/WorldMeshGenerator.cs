using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering.ProceduralModels;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WorldBuilding;
using WorldBuilding.Enums;
using WorldBuilding.WorldModel;

namespace Operation_HarmonyShift
{
    /// <summary>
    /// A procedural world mesh model
    /// </summary>
    [DataContract("WorldMeshGenerator")]
    [Display("WorldMesh")] // This name shows up in the procedural model dropdown list
    public class WorldMeshGenerator : PrimitiveProceduralModelBase
    {
        protected List<VertexPositionNormalTexture> vertices = new();
        protected List<int> indices = new();


        public WorldMeshGenerator(List<VertexPositionNormalTexture> verticesForMesh, List<int> indicesForMesh) : base()
        {
            vertices = verticesForMesh;
            indices = indicesForMesh;
        }

        /// <summary>
        /// Updates the vertex data, make sure <see cref="CreateVertexData"/> was called before.
        /// Can be overwirtten in subclass.
        /// </summary>
        protected virtual void UpdateVertexData()
        {
            // Update some vertices here. You can also re-generate everything... Totally up to you.
        }

        protected override GeometricMeshData<VertexPositionNormalTexture> CreatePrimitiveMeshData()
        {
            UpdateVertexData();
            // Create the primitive object for further processing by the base class
            return new GeometricMeshData<VertexPositionNormalTexture>(vertices.ToArray(), indices.ToArray(), isLeftHanded: false) { Name = "MyModel" };
        }
    }
}
