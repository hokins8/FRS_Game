using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int WorldHeight;
    public int ChunkSize;
    public List<Chunk> AllChunks = new();

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

        for (int i = 0; i < WorldHeight; i++)
        {
            Vector3 chunkPosition = new Vector3(transform.position.x, i * ChunkSize, transform.position.z);

            Chunk chunk = new Chunk(chunkPosition, grassMat);
            chunk.SpawnedChunk.transform.parent = this.transform;
            AllChunks.Add(chunk);
        }

        foreach (var chunk in AllChunks)
        {
            chunk.DrawChunk();
            yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(BuildWorldHeight());
    }
}