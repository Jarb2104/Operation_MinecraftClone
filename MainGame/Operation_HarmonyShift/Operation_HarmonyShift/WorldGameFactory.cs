using Operation_HarmonyShift;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldBuilding;
using WorldBuilding.Enums;
using WorldBuilding.WorldModel;

namespace StrideProceduralModel
{
    public class WorldGameFactory : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        // Declared public member fields and properties will show in the game studio
        [DataMember(1, "World Seed")]
        public int WorldSeed { get; set; } = 12022111;

        [DataMemberRange(1, 180, 1, 10, 0)]
        [DataMember(5, "Chunk Size")]
        public short ChunkSize { get; set; } = 1;

        [DataMemberRange(1, 9, 1, 1, 0)]
        [DataMember(5, "World Size")]
        //9x9 is the maximum number of chunks available as of now before the indices values go beyond what an int can hold
        public short WorldSize { get; set; } = 1;

        public Vector3 BlockScale { get; set; }
        public Material worldMaterial;

        protected World worldDescriptor = null;
        protected WorldMeshGenerator myModel;
        protected ModelComponent modelComponent;
        protected List<VertexPositionNormalTexture> vertices = new();
        protected List<int> indices = new();
        protected bool Init = false;

        //readonly ProfilingKey UpdateModelProfilingKey = new("Update My Model");

        public override async Task Execute()
        {
            Log.ActivateLog(LogMessageType.Fatal);
            // Setup the custom model
            if (!Init)
            {
                await CreateGameWorldMesh();
                Init = true;
            }

            //while (Game.IsRunning)
            //{
            //    // Do stuff every new frame
            //    await Script.NextFrame();
            //    using (Profiler.Begin(UpdateModelProfilingKey))
            //    {
            //        UpdateMyModel();
            //    }
            //}
        }

        async Task CreateGameWorldMesh()
        {
            //Generating world from parameters given coords
            Log.Debug("Calling the world creator");
            WorldGenerator creator = new(Log);
            var worldTask = Task.Run(() => creator.GenerateWorld(WorldSeed, WorldSize, 1, ChunkSize));

            await worldTask.ContinueWith((newGameWorld) =>
            {
                worldDescriptor = newGameWorld.Result;

                Log.Info("Generating world mesh");
                foreach (KeyValuePair<Vector3, Chunk> worldChunk in worldDescriptor.worldChunks)
                {
                    foreach (KeyValuePair<Vector3, Block> chunkBlock in worldChunk.Value.chunkBlocks)
                    {
                        if (chunkBlock.Value.IsTerrain)
                        {
                            CreateBlockFaces(chunkBlock.Value, worldChunk.Key);
                        }
                    }
                }

                Log.Info("Finished with world mesh");
            });

            await Task.Run(() =>
            {
                // The model classes
                Log.Info("Sending world mesh to the primitive procedural model base");
                myModel = new WorldMeshGenerator(vertices, indices);
                modelComponent = new ModelComponent(new Model());

                // Generate the procedual model
                myModel.Generate(Services, modelComponent.Model);

                // Add a meterial
                modelComponent.Model.Add(worldMaterial);

                // Add everything to the entity
                Entity.Add(modelComponent);
            });
        }

        //void UpdateMyModel()
        //{
        //    // Dispose previous vertex and index buffer
        //    modelComponent.Model.DisposeBuffers();

        //    // Create new model, so the entity processors pick it up
        //    modelComponent.Model = new Model
        //    {
        //        // Don't forget the material
        //        worldMaterial
        //    };

        //    // Re-generate the procedual model
        //    myModel.Generate(Services, modelComponent.Model);
        //}

        protected void CreateBlockFaces(Block currentBlock, Vector3 coords)
        {
            //Transform the vertices in the cube to accuratly reflect position and scale of the chunk
            currentBlock.ScaleBlock(BlockScale);
            currentBlock.TranslateBlock(new Vector3(coords.X * ChunkSize * BlockScale.X, 0 * ChunkSize * BlockScale.Y, coords.Z * ChunkSize * BlockScale.Z));

            //Add each of the cubes vertices to the mesh of the array a single time
            for (int i = 0; i < currentBlock.vertices.Count; i++)
            {
                vertices.Add(new VertexPositionNormalTexture(currentBlock.vertices[i], currentBlock.vertices[i], new Vector2(0, 0)));
            }

            foreach (FaceSide face in Enum.GetValues(typeof(FaceSide)))
            {
                Block neighborBlock = currentBlock.ParentChunk.GetNeighbor(currentBlock.BlockCoords, face);

                if (neighborBlock != null && !neighborBlock.IsTerrain)
                {
                    CreateFace(currentBlock.GetFaceVertices(face), face);
                }
                else if (neighborBlock == null)
                {
                    CreateFace(currentBlock.GetFaceVertices(face), face);
                }
            }
        }

        protected void CreateFace(List<Vector3> faceVertices, FaceSide face)
        {
            int firstVertex = vertices.Count - 8;
            switch (face)
            {
                case FaceSide.Back:
                case FaceSide.Right:
                case FaceSide.Bottom:
                    //First Triangle
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[1]));

                    //Second Triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[1]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    break;

                case FaceSide.Front:
                case FaceSide.Left:
                case FaceSide.Top:
                    //First Triangle               
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[1]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));

                    //Second Triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[2]));
                    break;
            }
        }
    }
}