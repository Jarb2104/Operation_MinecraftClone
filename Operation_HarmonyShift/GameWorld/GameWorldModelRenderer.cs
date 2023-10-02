using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Core.Shaders.Ast;
using System.Linq;

namespace Operation_HarmonyShift.GameWorld
{
    public class GameWorldModelRenderer : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
        public List<Material> WorldMaterials = new();

        private readonly Model GameWorldModel = new();
        private readonly Mesh GameWorldMesh = new();
        private readonly Buffer<Vector3> GameWorldVertexBuffer;
        private readonly Buffer<int> GameWorldIndexBuffer;

        public GameWorldModelRenderer(Entity gameWorldEntity, List<Vector3> vertices, List<int> indices) : base()
        {
            gameWorldEntity.Add(this);

            Mesh GameWorldMesh2 = new();
            Material GameWorldMaterial = Content.Load<Material>("Materials/Grass");
            GameWorldModel.Materials.Add(GameWorldMaterial);
            Material GameWorldMaterial2 = Content.Load<Material>("Materials/Dirt");
            GameWorldModel.Materials.Add(GameWorldMaterial2);
            
            GameWorldVertexBuffer = Buffer.Vertex.New(GraphicsDevice, vertices.ToArray(), GraphicsResourceUsage.Default);
            
            /* NOTE: if resource usage here  is set to immutable (the default) you will encounter an error if you try to update it after creating it */
            GameWorldIndexBuffer = Buffer.Index.New(GraphicsDevice, indices.ToArray());

            VertexDeclaration vertexDeclaration2 = VertexPositionTexture.Layout;
            VertexDeclaration vertexDeclaration = new(new VertexElement("POSITION", PixelFormat.R32G32B32_Float));

            GameWorldMesh = new Mesh
            {
                Draw = new MeshDraw
                {
                    PrimitiveType = PrimitiveType.TriangleList,
                    DrawCount = indices.Count,
                    IndexBuffer = new IndexBufferBinding(GameWorldIndexBuffer, true, indices.Count),
                    VertexBuffers = new[] { new VertexBufferBinding(GameWorldVertexBuffer, vertexDeclaration, GameWorldVertexBuffer.ElementCount) },
                }
            };

            var Vertices2 = vertices.Select(vpt => new VertexPositionTexture { Position = vpt + 1f });
            var GameWorldVertexBuffer2 = Buffer.Vertex.New(GraphicsDevice, Vertices2.ToArray(), GraphicsResourceUsage.Default);
            var GameWorldIndexBuffer2 = Buffer.Index.New(GraphicsDevice, indices.ToArray());

            GameWorldMesh2 = new Mesh
            {
                Draw = new MeshDraw
                {
                    PrimitiveType = PrimitiveType.TriangleList,
                    DrawCount = indices.Count,
                    IndexBuffer = new IndexBufferBinding(GameWorldIndexBuffer2, true, indices.Count),
                    VertexBuffers = new[] { new VertexBufferBinding(GameWorldVertexBuffer2, vertexDeclaration, GameWorldVertexBuffer2.ElementCount) },
                }
            };

            GameWorldMesh.MaterialIndex = 0;
            GameWorldModel.Add(GameWorldMesh);

            GameWorldMesh2.MaterialIndex = 1;
            GameWorldModel.Add(GameWorldMesh2);
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
