using System;
using System.Collections.Generic;
using Stride.Engine;
using Stride.Rendering;
using Stride.Graphics;
using WorldBuilding.Mathematics;
using WorldBuilding.Enums;
using Buffer = Stride.Graphics.Buffer;
using System.Linq;

namespace Operation_HarmonyShift.GameWorld
{
    public class GameWorldModelRenderer : SyncScript
    {
        Mesh GameWorldMesh;
        Buffer<VertexPositionNormal> GameWorldVertexBuffer;
        Buffer<int> GameWorldIndexBuffer;
        Model GameWorldModel = new();

        public GameWorldModelRenderer(Entity gameWorldEntity) : base()
        {
            gameWorldEntity.Add(this);
            
        }

        public void AddToGameWorld(Dictionary<BlockTypes, Material> cubeMaterials, Dictionary<BlockTypes, List<VertexPositionNormal>> modelVertices, Dictionary<BlockTypes, List<int>> modelIndices)
        {
            int matIndex = 0;
            foreach (BlockTypes blockType in Enum.GetValues(typeof(BlockTypes)))
            {
                if (blockType == BlockTypes.Air || !modelVertices.ContainsKey(blockType) || modelIndices[blockType].Count == 0)
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

            Entity worldChunkEntity = new()
            {
                Name = $"Chunk Entity {Entity.GetChildren().Count()}"
            };

            worldChunkEntity.GetOrCreate<ModelComponent>().Model = GameWorldModel;
            Entity.AddChild(worldChunkEntity);
        }

        public override void Update()
        {
        }
    }
}
