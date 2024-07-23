using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int WorldHeight;
    public int ChunkSize;
    public int WorldSize;
    public Dictionary<int, Chunk> AllChunks = new();

    private Material grassMat;

    public static World Instance;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator BuildWorldHeight()
    {
        var grassBlock = BlockCollection.Instance.GetBlock(BlockType.Grass);
        if (grassBlock != null)
            grassMat = grassBlock.GetMaterial();

        for (int z = 0; z < WorldSize; z++)
        {
            for (int x = 0; x < WorldSize; x++)
            {
                for (int y = 0; y < WorldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * ChunkSize, y * ChunkSize, z * ChunkSize);

                    Chunk chunk = new Chunk(chunkPosition, grassMat);
                    chunk.SpawnedChunk.transform.parent = this.transform;
                    //AllChunks.Add(chunk); // WIP -> generate ID?
                }
            }
        }
        

        foreach (var chunk in AllChunks)
        {
            chunk.Value.DrawChunk();
            yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(BuildWorldHeight());
    }
}