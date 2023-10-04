using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WorldBuilding;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;
using WorldBuilding.WorldDefinitions;
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
        [DataMember(5, "Chunk Size in Blocks")]
        public short ChunkSize { get; set; } = 1;

        [DataMemberRange(1, 20, 1, 1, 0)]
        [DataMember(5, "World Size in Chunks")]
        //9x9 is the maximum number of chunks available as of now before the indices values go beyond what an int can hold
        public short WorldSize { get; set; } = 1;

        [DataMemberRange(1, 512, 1, 1, 0)]
        [DataMember(5, "World Height in Blocks")]
        //9x9 is the maximum number of chunks available as of now before the indices values go beyond what an int can hold
        public short WorldHeight { get; set; } = 1;

        [DataMember(6, "Cube Materials")]
        public Dictionary<BlockType, Material> WorldMaterials = new();

        public Vector3 BlockScale { get; set; }

        private World worldDescriptor = null;
        private GameWorldModelRenderer GameWorldModel;
        private readonly Dictionary<BlockType, List<VertexPositionNormal>> verticesCollection = new();
        private readonly Dictionary<BlockType, List<int>> indicesCollection = new();
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

        private async Task CreateGameWorldMesh()
        {
            //Generating world from parameters given coords
            Log.Debug($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Calling the world creator");
            worldDescriptor = WorldGenerator.GenerateWorld(WorldSeed, WorldSize, (short)(WorldHeight / ChunkSize), ChunkSize, Log);

            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Generating world mesh");
            List<Task> chunksGeneration = worldDescriptor.worldChunks.Select(ck => CreateGameChunkMesh(ck)).ToList();
            await Task.WhenAll(chunksGeneration);
            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with world mesh");

            await Task.Run(() =>
            {
                // The model classes
                Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Sending world mesh to the primitive procedural model base");
                GameWorldModel = new GameWorldModelRenderer(Entity, WorldMaterials, verticesCollection, indicesCollection);
                GameWorldModel.Start();
            });
        }

        private async Task CreateGameChunkMesh(KeyValuePair<Vector3, Chunk> worldChunk)
        {
            Log.Warning($"{DateTime.Now.ToLongTimeString()} | Chunk {worldChunk.Key} terrain generation started");
            await worldChunk.Value.GenerateTerrain(WorldSeed, BlockScale.X, Log);
            Log.Verbose($"{DateTime.Now.ToLongTimeString()} | Chunk {worldChunk.Key} terrain generation finished");

            CreateGameBlockMesh(worldChunk.Value.chunkBlocks);

            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with chunk {worldChunk.Key} mesh");
        }

        private void CreateGameBlockMesh(Dictionary<Vector3, Block> chunkBlocks)
        {
            Span<Block> blocks = chunkBlocks.Select(vb => vb.Value).ToArray();
            foreach (Block chunkBlock in blocks)
            {
                //Transform the vertices in the cube to accuratly reflect position and scale of the chunk
                if (chunkBlock.IsTerrain)
                {
                    BlockType currentBlockType = chunkBlock.GetBlockType();
                    if (!verticesCollection.ContainsKey(currentBlockType))
                    {
                        verticesCollection.Add(currentBlockType, new List<VertexPositionNormal>());
                        indicesCollection.Add(currentBlockType, new List<int>());
                    }
                    CreateBlockFaces(chunkBlock, verticesCollection[currentBlockType], indicesCollection[currentBlockType]);
                }
            }
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

        private static void CreateBlockFaces(Block currentBlock, List<VertexPositionNormal> vertices, List<int> indices)
        {
            //Add each of the cubes vertices to the mesh of the array a single time
            List<Vector3> blockVertices = currentBlock.GetVertices();
            Block neighborBlock;
            for (int i = 0; i < blockVertices.Count; i++)
            {
                vertices.Add(new VertexPositionNormal(blockVertices[i], CubeHelpers.Normals[i]));
            }

            foreach (FaceSide face in Enum.GetValues(typeof(FaceSide)))
            {
                neighborBlock = currentBlock.ParentChunk.GetNeighbor(currentBlock.BlockCoords, face);
                neighborBlock ??= currentBlock.ParentChunk.GetNeighborFromNeighborChunk(currentBlock.BlockCoords, face);

                if (neighborBlock != null && !neighborBlock.IsTerrain)
                {
                    CreateFace(currentBlock.GetFaceVertices(face), face, vertices, indices);
                }
            }
        }

        private static void CreateFace(List<Vector3> faceVertices, FaceSide face, List<VertexPositionNormal> vertices, List<int> indices)
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