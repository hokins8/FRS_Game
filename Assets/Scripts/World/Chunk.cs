using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material chunkMaterial; // WIP

    private Block[,,] chunkData;

    public Block[,,] GetChunkData()
    {
        return chunkData;
    }

    IEnumerator buildChunk(int sizeX, int sizeY, int sizeZ)
    {
        chunkData = new Block[sizeX, sizeY, sizeZ];

        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    float rnd = Random.value;
                    if (rnd >= 0.65f)
                    {
                        var grassBlock = BlockCollection.Instance.GetBlock(BlockType.Rock);
                        if (grassBlock != null)
                            chunkData[x, y, z] = new Block(grassBlock.GetMaterial(), BlockType.Grass, pos, this);
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(null, BlockType.None, pos, this);
                    }
                }
            }
        }

        for (int z = 0; z < sizeZ; z++)
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    chunkData[x, y, z].Draw();

        CombineQuads();
        yield return null;
    }

    private void CombineQuads()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        MeshFilter mf = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);

        MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = chunkMaterial;

        foreach (Transform quad in transform)
            Destroy(quad.gameObject);
    }


    private void Start()
    {
        Invoke(nameof(BuildTestChunk), 1.5f);
    }

    private void BuildTestChunk()
    {
        StartCoroutine(buildChunk(10, 10, 10));
    }
}