using System;
using System.Collections.Generic;
using Stride.Engine;
using Stride.Rendering;
using Stride.Graphics;
using WorldBuilding.Mathematics;
using WorldBuilding.Enums;
using Buffer = Stride.Graphics.Buffer;

namespace Operation_HarmonyShift.GameWorld
{
    public class GameWorldModelRenderer : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
        private Mesh GameWorldMesh = new();
        private Buffer<VertexPositionNormal> GameWorldVertexBuffer;
        private Buffer<int> GameWorldIndexBuffer;

        private readonly Model GameWorldModel = new();
        private readonly Dictionary<BlockTypes, List<VertexPositionNormal>> VerticesCollection = new();
        private readonly Dictionary<BlockTypes, List<int>> IndicesCollection = new();
        private readonly Dictionary<BlockTypes, Material> CubeMaterials;

        public GameWorldModelRenderer(Entity gameWorldEntity, Dictionary<BlockTypes, Material> cubeMaterials, Dictionary<BlockTypes, List<VertexPositionNormal>> modelVertices, Dictionary<BlockTypes, List<int>> modelIndices) : base()
        {
            gameWorldEntity.Add(this);
            CubeMaterials = cubeMaterials;
            VerticesCollection = modelVertices;
            IndicesCollection = modelIndices;

            int matIndex = 0;
            foreach (BlockTypes blockType in Enum.GetValues(typeof(BlockTypes)))
            {
                if (!VerticesCollection.ContainsKey(blockType))
                    continue;

                GameWorldModel.Materials.Add(CubeMaterials[blockType]);

                /* NOTE: if resource usage here  is set to immutable (the default) you will encounter an error if you try to update it after creating it */
                GameWorldVertexBuffer = Buffer.Vertex.New(GraphicsDevice, VerticesCollection[blockType].ToArray(), GraphicsResourceUsage.Default);
                GameWorldIndexBuffer = Buffer.Index.New(GraphicsDevice, IndicesCollection[blockType].ToArray(), GraphicsResourceUsage.Default);

                GameWorldMesh = new Mesh
                {
                    Draw = new MeshDraw
                    {
                        PrimitiveType = PrimitiveType.TriangleList,
                        DrawCount = IndicesCollection[blockType].Count,
                        IndexBuffer = new IndexBufferBinding(GameWorldIndexBuffer, true, IndicesCollection[blockType].Count),
                        VertexBuffers = new[] { new VertexBufferBinding(GameWorldVertexBuffer, VertexPositionNormal.Layout, GameWorldVertexBuffer.ElementCount) },
                    },
                    MaterialIndex = matIndex++
                };
                GameWorldModel.Add(GameWorldMesh);
            }

            Entity.GetOrCreate<ModelComponent>().Model = GameWorldModel;
        }

        public override void Start()
        {
            // Initialization of the script.
            
        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}
