using Microsoft.VisualBasic.Logging;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldBuilding;
using WorldBuilding.Enums;
using WorldBuilding.Helpers;
using WorldBuilding.Mathematics;
using WorldBuilding.WorldModel;

namespace Operation_HarmonyShift.GameWorld
{
    public class GameWorldFactory : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        // Declared public member fields and properties will show in the game studio
        [DataMember(1, "World Seed")]
        public int WorldSeed { get; set; } = 12022111;

        [DataMemberRange(1, 32, 1, 10, 0)]
        [DataMember(2, "Chunk Width in Blocks")]
        public sbyte ChunkWidthSize { get; set; } = 1;

        [DataMemberRange(1, 32, 1, 10, 0)]
        [DataMember(3, "Chunk Length in Blocks")]
        public sbyte ChunkLengthSize { get; set; } = 1;

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
        private readonly Vector2[] EdgeTrackers =
        {
            new Vector2(0,0),
            new Vector2(0,0),
            new Vector2(0,0),
            new Vector2(0,0)
        };
        private readonly Vector2[] DrawDistance = new Vector2[4];
        private World worldDescriptor = null;
        private Dictionary<Vector2, GameWorldModelRenderer> GameWorldModels;
        private CameraComponent PlayerCamera;
        private Noise simplexNoise;
        private readonly Dictionary<BlockTypes, List<VertexPositionNormal>> verticesCollection = new();
        private readonly Dictionary<BlockTypes, List<int>> indicesCollection = new();
        private readonly ProfilingKey UpdateModelProfilingKey = new("World Factory Model Population");

        private struct MeshDefinition
        {
            public List<VertexPositionNormal> meshVertices;
            public List<int> meshIndices;

            public MeshDefinition()
            {
                meshVertices = new List<VertexPositionNormal>();
                meshIndices = new List<int>();
            }
        }



        public override async Task Execute()
        {

            Log.ActivateLog(LogMessageType.Debug);
            // Setup the custom model
            if (!Init)
            {
                //Creating class to generate noise
                simplexNoise = new(WorldSeed);
                //Getting the camera player
                PlayerCamera = FindCameraComponent(PlayerCameraName);
                //Creating the dictionary for the world models
                if (GameWorldModels == null)
                {
                    GameWorldModels = new();
                }
                //Setting the draw distance for each direction of the map
                DrawDistance[0] = new Vector2(ViewDistance, ViewDistance);
                DrawDistance[1] = new Vector2(ViewDistance, -ViewDistance);
                DrawDistance[2] = new Vector2(-ViewDistance, -ViewDistance);
                DrawDistance[3] = new Vector2(-ViewDistance, ViewDistance);

                Init = true;
            }

            while (Game.IsRunning)
            {
                await Script.NextFrame();
                // Do stuff every new frame

                using (Profiler.Begin(UpdateModelProfilingKey))
                {
                    foreach (Directions direction in Enum.GetValues(typeof(Directions)))
                    {
                        DrawWorld(direction, simplexNoise);
                    }
                }
            }
        }

        private async void DrawWorld(Directions direction, Noise simplexNoise)
        {
            float currentViewDistance = Vector2.Distance(PlayerCamera.GetWorldPosition().XZ(), EdgeTrackers[(sbyte)direction]);
            float actualViewDistance = Vector2.Distance(PlayerCamera.GetWorldPosition().XZ(), DrawDistance[(sbyte)direction]);
            if (currentViewDistance < actualViewDistance)
            {
                await CreateGameWorldMeshPortion(new Vector2(0, EdgeTrackers[(sbyte)direction].Y), DrawDistance[(sbyte)direction], simplexNoise);
                await CreateGameWorldMeshPortion(new Vector2(EdgeTrackers[(sbyte)direction].X, 0), DrawDistance[(sbyte)direction], simplexNoise);
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

        private async Task CreateGameWorldMeshPortion(Vector2 initPosition, Vector2 endPosition, Noise simplexNoise)
        {
            //Generating world from parameters given coords
            Log.Debug($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Calling the world creator");
            worldDescriptor = WorldGenerator.GenerateWorld(WorldSeed, WorldHeight, ChunkWidthSize, ChunkLengthSize, Log);

            worldDescriptor.CreateWorldChunks(Log, initPosition, endPosition);

            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Generating world");
            List<Task> chunkGenerators = worldDescriptor.worldChunks.Select(ck => ck.Value.GenerateTerrain(BlockScale.X, WorldHeight, simplexNoise)).ToList();
            await Task.WhenAll(chunkGenerators);
            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with world generation");

            Log.Debug($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Populating the world's mesh with vertices and indexes");
            await Parallel.ForEachAsync(worldDescriptor.worldChunks, async (ck, CancellationToken) =>
            {
                Dictionary<BlockTypes, MeshDefinition> meshResult = await Task.Run(() => CreateChunkBlocks(ck.Value.ChunkBlocks));
                Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Populating {ck.Key} vertices and indices");
                lock (verticesCollection)
                {
                    foreach (KeyValuePair<BlockTypes, MeshDefinition> chunkMeshPortion in meshResult)
                    {
                        if (!verticesCollection.ContainsKey(chunkMeshPortion.Key))
                        {
                            verticesCollection.Add(chunkMeshPortion.Key, new List<VertexPositionNormal>());
                            indicesCollection.Add(chunkMeshPortion.Key, new List<int>());
                        }

                        indicesCollection[chunkMeshPortion.Key].AddRange(chunkMeshPortion.Value.meshIndices.Select(index => index + verticesCollection[chunkMeshPortion.Key].Count));
                        verticesCollection[chunkMeshPortion.Key].AddRange(chunkMeshPortion.Value.meshVertices);
                    }
                }
            });
            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished populating the world's mesh with vertices and indexes");
            await Task.Run(() =>
            {
                // The model classes
                Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Sending world mesh to the primitive procedural model base");
                GameWorldModels.Add(initPosition, new GameWorldModelRenderer(Entity, WorldMaterials, verticesCollection, indicesCollection));
            });
        }

        private static Dictionary<BlockTypes, MeshDefinition> CreateChunkBlocks(Dictionary<Vector3SByte, Block> chunkBlocks)
        {
            Dictionary<BlockTypes, MeshDefinition> chunkMesh = new();
            Span<Block> blocks = chunkBlocks.Select(vb => vb.Value).ToArray();
            Block neighborBlock;
            bool isVisible = false;

            foreach (Block chunkBlock in blocks)
            {
                isVisible = false;
                foreach (FaceSides face in Enum.GetValues(typeof(FaceSides)))
                {
                    neighborBlock = chunkBlock.ParentChunk.GetNeighbor(chunkBlock.BlockCoords, face);
                    neighborBlock ??= chunkBlock.ParentChunk.GetNeighborFromNeighborChunk(chunkBlock.BlockCoords, face);

                    if (neighborBlock != null && !neighborBlock.IsTerrain)
                    {
                        isVisible = true;
                        break;
                    }
                    else if (neighborBlock == null)
                    {
                        isVisible = true;
                        break;
                    }
                }

                if (chunkBlock.IsTerrain && isVisible)
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
            Block neighborBlock;

            vertices.AddRange(blockVertices.Select(v => new VertexPositionNormal(v, CubeHelpers.Normals[blockVertices.IndexOf(v)])));

            foreach (FaceSides face in Enum.GetValues(typeof(FaceSides)))
            {
                neighborBlock = currentBlock.ParentChunk.GetNeighbor(currentBlock.BlockCoords, face);
                neighborBlock ??= currentBlock.ParentChunk.GetNeighborFromNeighborChunk(currentBlock.BlockCoords, face);

                if (neighborBlock != null && !neighborBlock.IsTerrain)
                {
                    CreateFace(currentBlock.GetFaceVertices(face), face, vertices, indices);
                }
                else if (neighborBlock == null)
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
    }
}