using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
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
        [DataMember(5, "Chunk Width in Blocks")]
        public sbyte ChunkWidthSize { get; set; } = 1;

        [DataMemberRange(1, 32, 1, 10, 0)]
        [DataMember(5, "Chunk Length in Blocks")]
        public sbyte ChunkLengthSize { get; set; } = 1;

        [DataMemberRange(1000, 1000000, 1, 100, 0)]
        [DataMember(5, "World Size in Chunks")]
        public short WorldSize { get; set; } = 1;

        [DataMemberRange(1, 100, 1, 1, 0)]
        [DataMember(5, "Visible World Size in Chunks")]
        public short VisbleWorldSize { get; set; } = 1;

        [DataMemberRange(1, 512, 1, 1, 0)]
        [DataMember(5, "World Height in Blocks")]
        public short WorldHeight { get; set; } = 1;

        [DataMember(6, "Cube Materials")]
        public Dictionary<BlockTypes, Material> WorldMaterials = new();
        public Vector3 BlockScale { get; set; }

        private World worldDescriptor = null;
        private GameWorldModelRenderer GameWorldModel;
        private readonly Dictionary<BlockTypes, List<VertexPositionNormal>> verticesCollection = new();
        private readonly Dictionary<BlockTypes, List<int>> indicesCollection = new();
        private bool Init = false;

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
            worldDescriptor = WorldGenerator.GenerateWorld(WorldSeed, VisbleWorldSize, WorldHeight, ChunkWidthSize, ChunkLengthSize, Log);
            Noise simplexNoise = new(WorldSeed);
            Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Generating world");
            List<Task> chunkGenerators = worldDescriptor.worldChunks.Select(ck => CreateGameChunk(ck, simplexNoise)).ToList();
            await Task.WhenAll(chunkGenerators);
            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with world generation");

            Log.Debug($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Populating the world's mesh with vertices and indexes");
            await Parallel.ForEachAsync(worldDescriptor.worldChunks, async (ck, CancellationToken) =>
            {
                Dictionary<BlockTypes, MeshDefinition> meshResult = await CreateGameChunkMesh(ck);
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
            Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with world mesh generation");
            await Task.Run(() =>
            {
                // The model classes
                Log.Verbose($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Sending world mesh to the primitive procedural model base");
                GameWorldModel = new GameWorldModelRenderer(Entity, WorldMaterials, verticesCollection, indicesCollection);
            });
        }

        private async Task CreateGameChunk(KeyValuePair<Vector3, Chunk> worldChunk, Noise simplexNoise)
        {
            //Log.Warning($"{DateTime.Now.ToLongTimeString()} | Chunk {worldChunk.Key} terrain generation started");
            await worldChunk.Value.GenerateTerrain(BlockScale.X, WorldHeight, simplexNoise, Log);
            //Log.Info($"{DateTime.Now.ToLongTimeString()} | Chunk {worldChunk.Key} terrain generation finished");
        }

        private static async Task<Dictionary<BlockTypes, MeshDefinition>> CreateGameChunkMesh(KeyValuePair<Vector3, Chunk> worldChunk)
        {
            //Log.Warning($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Started with chunk {worldChunk.Key} mesh");
            Dictionary<BlockTypes, MeshDefinition> result = await Task.Run(() => CreateGameBlocksVertices(worldChunk.Value.chunkBlocks));
            ////Log.Info($"TimeStamp: {DateTime.Now.ToLongTimeString()} | Finished with chunk {worldChunk.Key} mesh");
            return result;
        }

        private static Dictionary<BlockTypes, MeshDefinition> CreateGameBlocksVertices(Dictionary<Vector3SByte, Block> chunkBlocks)
        {
            Dictionary<BlockTypes, MeshDefinition> chunkMesh = new();
            Span<Block> blocks = chunkBlocks.Select(vb => vb.Value).ToArray();
            foreach (Block chunkBlock in blocks)
            {
                var isVisible = false;
                foreach (FaceSide face in Enum.GetValues(typeof(FaceSide)))
                {
                    var neighborBlock = chunkBlock.ParentChunk.GetNeighbor(chunkBlock.BlockCoords, face);
                    neighborBlock ??= chunkBlock.ParentChunk.GetNeighborFromNeighborChunk(chunkBlock.BlockCoords, face);

                    isVisible = neighborBlock != null && !neighborBlock.IsTerrain || neighborBlock != null;
                    if (isVisible) break;
                }

                if (chunkBlock.IsTerrain)
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

            vertices.AddRange(blockVertices.Select(v => new VertexPositionNormal(v, CubeHelpers.Normals[blockVertices.IndexOf(v)])));

            foreach (FaceSide face in Enum.GetValues(typeof(FaceSide)))
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

        private static void CreateFace(List<Vector3> faceVertices, FaceSide face, List<VertexPositionNormal> vertices, List<int> indices)
        {
            int firstVertex = vertices.Count - 8;
            switch (face)
            {
                case FaceSide.Front:
                case FaceSide.Left:
                case FaceSide.Top:

                    //First Triangle               
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[2]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));

                    //Second Triangle              
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[1]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[3]));
                    indices.Add(vertices.FindIndex(firstVertex, v => v.Position == faceVertices[0]));
                    break;

                case FaceSide.Back:
                case FaceSide.Right:
                case FaceSide.Bottom:
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