using UnityEngine;

public class Chunk
{
    public Material ChunkMaterial;
    public GameObject SpawnedChunk;

    private Block[,,] chunkData;

    public Block[,,] GetChunkData()
    {
        return chunkData;
    }

    private void BuildChunk()
    {
        int worldSize = World.Instance.ChunkSize;
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

                    if (wY <= PerlinNoise.Instance.GenerateHeight(wX, wZ))
                        chunkData[x, y, z] = new Block(BlockType.Grass, pos, this);
                    else
                        chunkData[x, y, z] = new Block(BlockType.None, pos, this);
                    //float rnd = Random.value;
                    //if (rnd >= 0.65f)
                    //    chunkData[x, y, z] = new Block(BlockType.Grass, pos, this);
                    //else
                    //    chunkData[x, y, z] = new Block(BlockType.None, pos, this);
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
        int worldSize = World.Instance.ChunkSize;

        for (int z = 0; z < worldSize; z++)
            for (int y = 0; y < worldSize; y++)
                for (int x = 0; x < worldSize; x++)
                    chunkData[x, y, z].Draw();

        CombineQuads();
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

        MeshFilter mf = (MeshFilter)SpawnedChunk.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer meshRenderer = SpawnedChunk.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = ChunkMaterial;

        foreach (Transform quad in SpawnedChunk.transform)
            GameObject.Destroy(quad.gameObject);
    }
}