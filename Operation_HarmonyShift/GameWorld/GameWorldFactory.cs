using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldBuilding;
using WorldBuilding.Enums;
using WorldBuilding.WorldModel;

namespace Operation_HarmonyShift.GameWorld
{
    public class GameWorldFactory : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        // Declared public member fields and properties will show in the game studio
        [DataMember(1, "World Seed")]
        public int WorldSeed { get; set; } = 12022111;

        [DataMemberRange(1, 64, 1, 10, 0)]
        [DataMember(5, "Chunk Size")]
        public short ChunkSize { get; set; } = 1;

        [DataMemberRange(1, 20, 1, 1, 0)]
        [DataMember(5, "World Size")]
        //9x9 is the maximum number of chunks available as of now before the indices values go beyond what an int can hold
        public short WorldSize { get; set; } = 1;

        public Vector3 BlockScale { get; set; }

        private World worldDescriptor = null;
        private GameWorldModelRenderer GameWorldModel;
        private readonly List<Vector3> vertices = new();
        private readonly List<int> indices = new();
        private bool Init = false;

        //readonly ProfilingKey UpdateModelProfilingKey = new("Update My Model");

        public override async Task Execute()
        {
            Log.ActivateLog(LogMessageType.Debug);
            // Setup the custom model
            if (!Init)
            {
                await CreateGameWorldMesh();
                Init = true;
            }

            //while (Game.IsRunning)
            //{
            //    Log.Info("Test3");
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
            Log.Debug($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Calling the world creator");
            var worldTask = Task.Run(() => WorldGenerator.GenerateWorld(WorldSeed, WorldSize, 1, ChunkSize, Log));

            await worldTask.ContinueWith((newGameWorld) =>
            {
                worldDescriptor = newGameWorld.Result;

                Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Generating world mesh");
                foreach (KeyValuePair<Vector3, Chunk> worldChunk in worldDescriptor.worldChunks)
                {
                    foreach (KeyValuePair<Vector3, Block> chunkBlock in worldChunk.Value.chunkBlocks)
                    {
                        //Transform the vertices in the cube to accuratly reflect position and scale of the chunk
                        chunkBlock.Value.ScaleBlock(BlockScale);
                        chunkBlock.Value.TranslateBlock(new Vector3(worldChunk.Value.ChunkCoords.X * BlockScale.X, worldChunk.Value.ChunkCoords.Y * BlockScale.Y, worldChunk.Value.ChunkCoords.Z * BlockScale.Z) * ChunkSize);
                        if (chunkBlock.Value.IsTerrain)
                        {
                            CreateBlockFaces(chunkBlock.Value);
                        }
                    }
                    Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with chunk {worldChunk.Key} mesh");
                }

                Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with world mesh");
            });

            await Task.Run(() =>
            {
                // The model classes
                Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Sending world mesh to the primitive procedural model base");
                GameWorldModel = new GameWorldModelRenderer(Entity, vertices, indices);
                GameWorldModel.Start();
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

        protected void CreateBlockFaces(Block currentBlock)
        {
            //Add each of the cubes vertices to the mesh of the array a single time
            for (int i = 0; i < currentBlock.vertices.Count; i++)
            {
                vertices.Add(currentBlock.vertices[i]);
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
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[1]));

                    //Second Triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[1]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[3]));
                    break;

                case FaceSide.Front:
                case FaceSide.Left:
                case FaceSide.Top:
                    //First Triangle               
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[1]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[3]));

                    //Second Triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v == faceVertices[2]));
                    break;
            }
        }
    }
}