using Microsoft.VisualBasic.Logging;
using SharpDX.MediaFoundation;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WorldBuilding;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;
using WorldBuilding.Mathematics;
using WorldBuilding.WorldDefinitions;
using WorldBuilding.WorldModel;

namespace Operation_HarmonyShift.GameWorld
{
    public class GameWorldFactory : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        [DataMember(1, "World Seed")]
        public int WorldSeed { get; set; } = 12022111;

        [DataMemberRange(1, 32, 1, 10, 0)]
        [DataMember(2, "Chunk Size in Blocks")]
        public sbyte ChunkSize { get; set; } = 1;

        [DataMemberRange(1, 512, 1, 1, 0)]
        [DataMember(4, "World Height in Blocks")]
        public short WorldHeight { get; set; } = 1;

        [DataMemberRange(100, 1000000, 100, 10000, 0)]
        [DataMember(5, "Visible World")]
        public int ViewDistance { get; set; } = 1;

        [DataMember(6, "Cube Materials")]
        public Dictionary<BlockTypes, Material> WorldMaterials = new();
        public Vector3 BlockScale { get; set; }

        [DataMember(7, "Player Camera Entity Name")]
        public string PlayerCameraName;

        private bool Init = false;
        private int ModelSize;
        private FastNoiseLite fastNoise;
        private World worldDescriptor = null;
        private CameraComponent PlayerCamera;
        private ConcurrentDictionary<Vector3, ModelMeshDefinition> GameWorldModels;
        private GameWorldModelRenderer GameWorldModel;


        private readonly sbyte ChunksPerEntity = 2;
        private readonly ProfilingKey UpdateModelProfilingKey = new("World Factory Model Population");
        private readonly Vector2[] DrawDistance = new Vector2[4];

        private struct MeshDefinition
        {
            public List<VertexPositionNormal> meshVertices;
            public List<int> meshIndices;

            public MeshDefinition()
            {
                meshVertices = new();
                meshIndices = new();
            }
        }

        private struct ModelMeshDefinition
        {
            public Dictionary<BlockTypes, List<VertexPositionNormal>> verticesCollection;
            public Dictionary<BlockTypes, List<int>> indicesCollection;

            public ModelMeshDefinition()
            {
                verticesCollection = new();
                indicesCollection = new();
            }
        }

        public override async Task Execute()
        {

            Log.ActivateLog(LogMessageType.Debug);
            // Setup the custom model
            if (!Init)
            {
                //Creating class to generate noise
                fastNoise = new(WorldSeed);
                fastNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                //Getting the camera player
                PlayerCamera = FindCameraComponent(PlayerCameraName);

                //Setting the size of each model for the terrain
                ModelSize = ChunksPerEntity * ChunkSize;

                //Initializing dictionary to store the entities with models.
                GameWorldModels = new ConcurrentDictionary<Vector3, ModelMeshDefinition>();
                GameWorldModel = new(Entity);

                //Setting the draw distance for each direction of the map
                DrawDistance[0] = new Vector2(ViewDistance, ViewDistance);
                DrawDistance[1] = new Vector2(ViewDistance, -ViewDistance);
                DrawDistance[2] = new Vector2(-ViewDistance, -ViewDistance);
                DrawDistance[3] = new Vector2(-ViewDistance, ViewDistance);

                //Creating world from given parameters
                Log.Debug($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Calling the world creator");
                worldDescriptor = WorldGenerator.GenerateWorld(WorldSeed, WorldHeight, ChunkSize, Log);
                Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | World created");
                GenerateWorldData();

                Init = true;
                Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished init!");
            }

            while (Game.IsRunning)
            {
                await Script.NextFrame();
                // Do stuff every new frame
                using (Profiler.Begin(UpdateModelProfilingKey))
                {
                    Log.Warning($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Check if Alive!");
                    //foreach (Directions direction in Enum.GetValues(typeof(Directions)))
                    //{
                    //    // The model classes
                    //    await DrawWorld(direction, fastNoise);
                    //}
                    //await DrawWorld(Directions.East, fastNoise);
                    //await DrawWorld(Directions.South, fastNoise);
                    //await DrawWorld(Directions.West, fastNoise);
                }
            }
        }

        private CameraComponent FindCameraComponent(string cameraComponentName)
        {
            foreach (var entity in Entity.Scene.Entities)
            {
                if (entity.Name == cameraComponentName)
                {
                    return entity.Get<CameraComponent>();
                }
            }

            return null;
        }

        private void GenerateWorldData()
        {
            int chunkMatrixNM = ViewDistance / ChunkSize;
            int matrixChunksTotal = (int)Math.Pow(chunkMatrixNM, 2);
            Vector3 playerPos;
            Vector2 movementDirection;
            float tempDirection;

            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Creating world chunks");
            for (int j = WorldConstants.WorldHeightInChunks - 1; j >= 0; j--)
            {
                playerPos = Vector3.Zero;
                playerPos.Y = j;
                movementDirection.X = 0;
                movementDirection.Y = -1;
                for (int i = 0; i < matrixChunksTotal; i++)
                {
                    Chunk newChunk = new(worldDescriptor, playerPos, ChunkSize, worldDescriptor.ChunkHeightSize);
                    worldDescriptor.worldChunks.Add(playerPos, newChunk);
                    if (playerPos.X == playerPos.Z ||
                        playerPos.X < 0 && playerPos.X == -playerPos.Z ||
                        playerPos.X > 0 && playerPos.X == 1 - playerPos.Z)
                    {
                        tempDirection = movementDirection.X;
                        movementDirection.X = -movementDirection.Y;
                        movementDirection.Y = tempDirection;
                    }
                    playerPos.X += movementDirection.X;
                    playerPos.Z += movementDirection.Y;
                }
            };
            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with chunks creation");

            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Generating blocks for the chunks");
            ParallelQuery<KeyValuePair<Vector3, Chunk>> chunkWorkers = worldDescriptor.worldChunks.AsParallel().WithDegreeOfParallelism(chunkMatrixNM);
            chunkWorkers.ForAll(GenerateChunkBlocksData);
            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with blocks for the chunks");

            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Checking blocks visibility and creating visible chunks and blocks");
            chunkWorkers.ForAll(CheckChunksVisibility);
            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with processing initial world loading");

            Span<KeyValuePair<Vector3, ModelMeshDefinition>> populateToWorld = GameWorldModels.ToArray();
            foreach(KeyValuePair<Vector3, ModelMeshDefinition> chunkModel in populateToWorld)
            {
                Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Sending world chunk {chunkModel.Key} to the world");
                GameWorldModel.AddToGameWorld(WorldMaterials, chunkModel.Value.verticesCollection, chunkModel.Value.indicesCollection);
            }
            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished all processes");
        }

        private void GenerateChunkBlocksData(KeyValuePair<Vector3, Chunk> newChunk)
        {
            newChunk.Value.GenerateTerrain(BlockScale.X, WorldHeight, fastNoise);
        }

        private void CheckChunksVisibility(KeyValuePair<Vector3, Chunk> newChunk)
        {
            if (newChunk.Value.CheckVisibility())
            {
                Dictionary<BlockTypes, MeshDefinition> meshResult = CreateChunkBlocks(newChunk.Value.ChunkBlocks);
                ModelMeshDefinition newMeshModel = new();
                GenerateCompileVertexIndices(meshResult, ref newMeshModel.verticesCollection, ref newMeshModel.indicesCollection);
                GameWorldModels.AddOrUpdate(newChunk.Key, newMeshModel, (key, oldModel) => oldModel);
            }
        }

        private static Dictionary<BlockTypes, MeshDefinition> CreateChunkBlocks(Dictionary<Vector3SByte, Block> chunkBlocks)
        {
            Dictionary<BlockTypes, MeshDefinition> chunkMesh = new();
            Span<Block> blocks = chunkBlocks.Select(vb => vb.Value).ToArray();

            foreach (Block chunkBlock in blocks)
            {
                if (chunkBlock.IsVisible)
                {
                    if (!chunkMesh.ContainsKey(chunkBlock.BlockType))
                    {
                        chunkMesh.Add(chunkBlock.BlockType, new MeshDefinition());
                    }
                    CreateBlockFaces(chunkBlock, chunkMesh[chunkBlock.BlockType].meshVertices, chunkMesh[chunkBlock.BlockType].meshIndices);
                }
            }
            return chunkMesh;
        }

        private static void CreateBlockFaces(Block currentBlock, List<VertexPositionNormal> vertices, List<int> indices)
        {
            //Add each of the cubes vertices to the mesh of the array a single time
            List<Vector3> blockVertices = currentBlock.GetVertices();
            Block neighborBlock = null;

            vertices.AddRange(blockVertices.Select(v => new VertexPositionNormal(v, CubeHelpers.Normals[blockVertices.IndexOf(v)])));

            foreach (FaceSides face in Enum.GetValues(typeof(FaceSides)))
            {
                neighborBlock = currentBlock.ParentChunk.GetNeighbor(currentBlock.BlockCoords, face);
                neighborBlock ??= currentBlock.ParentChunk.GetNeighborFromNeighborChunk(currentBlock.BlockCoords, face);

                if (neighborBlock == null || neighborBlock.BlockType == BlockTypes.Air)
                {
                    CreateFace(currentBlock.GetFaceVertices(face), face, vertices, indices);
                }
            }
        }

        private static void CreateFace(List<Vector3> faceVertices, FaceSides face, List<VertexPositionNormal> vertices, List<int> indices)
        {
            int firstVertex = vertices.Count - 8;
            switch (face)
            {
                case FaceSides.Front:
                case FaceSides.Left:
                case FaceSides.Top:
                    //First Triangle               
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));

                    //Second Triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[1]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    break;

                case FaceSides.Back:
                case FaceSides.Right:
                case FaceSides.Bottom:
                    //first triangle               
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));

                    //second triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[1]));
                    break;
            }
        }

        private void GenerateCompileVertexIndices(Dictionary<BlockTypes, MeshDefinition> meshResults, ref Dictionary<BlockTypes, List<VertexPositionNormal>> verticesCollection, ref Dictionary<BlockTypes, List<int>> indicesCollection)
        {
            Span<KeyValuePair<BlockTypes, MeshDefinition>> meshParts = meshResults.ToArray();
            int vertexCount;

            for (int i = 0; i < meshParts.Length; i++)
            {
                if (!verticesCollection.ContainsKey(meshParts[i].Key))
                {
                    verticesCollection.Add(meshParts[i].Key, new List<VertexPositionNormal>());
                    indicesCollection.Add(meshParts[i].Key, new List<int>());
                }

                vertexCount = verticesCollection[meshParts[i].Key].Count;
                indicesCollection[meshParts[i].Key].AddRange(meshParts[i].Value.meshIndices.Select(index => index + vertexCount));
                verticesCollection[meshParts[i].Key].AddRange(meshParts[i].Value.meshVertices);
            };
        }
    }
}