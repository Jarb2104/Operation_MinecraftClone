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
        private readonly Model GameWorldModel = new();
        private readonly Mesh GameWorldMesh = new();
        private readonly Buffer<VertexPositionNormal> GameWorldVertexBuffer;
        private readonly Buffer<int> GameWorldIndexBuffer;

        public GameWorldModelRenderer(Entity gameWorldEntity, Dictionary<BlockType, Material> cubeMaterials, Dictionary<BlockType, List<VertexPositionNormal>> modelVertices, Dictionary<BlockType, List<int>> modelIndices) : base()
        {
            gameWorldEntity.Add(this);
            int matIndex = 0;

            foreach (BlockType blockType in Enum.GetValues(typeof(BlockType)))
            {
                if (!modelVertices.ContainsKey(blockType))
                    continue;

                GameWorldModel.Materials.Add(cubeMaterials[blockType]);

                /* NOTE: if resource usage here  is set to immutable (the default) you will encounter an error if you try to update it after creating it */
                GameWorldVertexBuffer = Buffer.Vertex.New(GraphicsDevice, modelVertices[blockType].ToArray(), GraphicsResourceUsage.Default);
                GameWorldIndexBuffer = Buffer.Index.New(GraphicsDevice, modelIndices[blockType].ToArray(), GraphicsResourceUsage.Default);

                GameWorldMesh = new Mesh
                {
                    Draw = new MeshDraw
                    {
                        PrimitiveType = PrimitiveType.TriangleList,
                        DrawCount = modelIndices[blockType].Count,
                        IndexBuffer = new IndexBufferBinding(GameWorldIndexBuffer, true, modelIndices[blockType].Count),
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
