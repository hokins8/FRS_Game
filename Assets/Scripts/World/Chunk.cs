using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum ChunkStatus
{
    ReadyToDraw = 0,
    Done = 1,
}

[Serializable]
class BlockData
{
    private BlockType[,,] matrix;

    public BlockData() { }

    public BlockData(Block[,,] block)
    {
        var chunkSize = World.Instance.GetChunkSize();
        matrix = new BlockType[chunkSize, chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++)
            for (int y = 0; y < chunkSize; y++)
                for (int z = 0; z < chunkSize; z++)
                    matrix[x, y, z] = block[x, y, z].GetBlockType();

    }

    public BlockType[,,] GetMatrix()
    {
        return matrix;
    }
}

public class Chunk
{
    public Material ChunkMaterial;
    public GameObject SpawnedChunk;
    public ChunkStatus ChunkStatus;

    private Block[,,] chunkData;
    private BlockData blockData;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;

    public Block[,,] GetChunkData()
    {
        return chunkData;
    }

    private string BuildChunkFileName(Vector3 pos)
    {
        var mainMenu = MainMenu_UI.Instance;
        return $"{mainMenu.SavesLocation}\\{mainMenu.CurrentFolder}/Chunk_{(int)pos.x}_{(int)pos.y}_{(int)pos.z}_{World.Instance.GetChunkSize()}_{World.Instance.GetWorldRadius()}.dat";
    }

    private bool Load()
    {
        string chunkFile = BuildChunkFileName(SpawnedChunk.transform.position);
        if (File.Exists(chunkFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.Open);
            blockData = new BlockData();
            blockData = (BlockData)bf.Deserialize(file);
            file.Close();
            return true;
        }
        return false;
    }

    public void Save()
    {
        if (SpawnedChunk != null)
        {
            string chunkFile = BuildChunkFileName(SpawnedChunk.transform.position);
            if (!File.Exists(chunkFile))
                Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
            blockData = new BlockData(chunkData);
            bf.Serialize(file, blockData);
            file.Close();

            Player.Instance.PlayerInventory.SaveInventory();
        }
    }

    private void BuildChunk()
    {
        bool loadedData = Load();

        int worldSize = World.Instance.GetChunkSize();
        chunkData = new Block[worldSize, worldSize, worldSize];

        for (int z = 0; z < worldSize; z++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int x = 0; x < worldSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    int wX = (int)(x + SpawnedChunk.transform.position.x);
                    int wY = (int)(y + SpawnedChunk.transform.position.y);
                    int wZ = (int)(z + SpawnedChunk.transform.position.z);

                    if (loadedData)
                    {
                        chunkData[x, y, z] = new Block(blockData.GetMatrix()[x, y, z], pos, this);
                        continue;
                    }

                    if (wY <= 0)
                        chunkData[x, y, z] = new Block(BlockType.Floor, pos, this);
                    else if (wY >= 140)
                        chunkData[x, y, z] = new Block(BlockType.Floor, pos, this);
                    else if (PerlinNoise.Instance.GenerateCaves(wX,wY, wZ, 0.1f, 3) < 0.44f)
                        chunkData[x, y, z] = new Block(BlockType.None, pos, this);
                    else if (wY <= PerlinNoise.Instance.GenerateRockHeight(wX, wZ))
                        chunkData[x, y, z] = new Block(BlockType.Rock, pos, this);
                    else if (wY <= PerlinNoise.Instance.GenerateGrassHeight(wX, wZ))
                        chunkData[x, y, z] = new Block(BlockType.Grass, pos, this);
                    else if (wY <= PerlinNoise.Instance.GenerateSnowHeight(wX, wZ))
                        chunkData[x, y, z] = new Block(BlockType.Snow, pos, this);
                    else
                        chunkData[x, y, z] = new Block(BlockType.None, pos, this);

                    ChunkStatus = ChunkStatus.ReadyToDraw;
                }
            }
        }
    }

    public Chunk(Vector3 pos, Material material)
    {
        SpawnedChunk = new GameObject(World.Instance.SetChunkNameByPos(pos));
        SpawnedChunk.transform.position = pos;
        ChunkMaterial = material;
        BuildChunk();
    }

    public void DrawChunk()
    {
        int worldSize = World.Instance.GetChunkSize();

        for (int z = 0; z < worldSize; z++)
            for (int y = 0; y < worldSize; y++)
                for (int x = 0; x < worldSize; x++)
                    chunkData[x, y, z].Draw();

        CombineQuads();

        collider = SpawnedChunk.AddComponent<MeshCollider>();
        collider.sharedMesh = meshFilter.mesh;
    }

    public void Redraw()
    {
        GameObject.DestroyImmediate(meshFilter);
        GameObject.DestroyImmediate(meshRenderer);
        GameObject.DestroyImmediate(collider);
        DrawChunk();
    }

    private void CombineQuads()
    {
        MeshFilter[] meshFilters = SpawnedChunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        meshFilter = SpawnedChunk.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();

        meshFilter.mesh.CombineMeshes(combine);

        meshRenderer = SpawnedChunk.AddComponent<MeshRenderer>();
        meshRenderer.material = ChunkMaterial;

        foreach (Transform quad in SpawnedChunk.transform)
            GameObject.Destroy(quad.gameObject);
    }
}