using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] Material matAtlas;

    [Space]
    [Header("World Definition")]
    [SerializeField] int worldHeight;
    [SerializeField] int chunkSize;
    [SerializeField] int worldRadius;

    [Space]
    [Header("Player")]
    [SerializeField] Player player;

    public Dictionary<string, Chunk> AllChunks = new();
    
    public static World Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(BuildWorldHeight());
    }

    public string SetChunkNameByPos(Vector3 pos)
    {
        return pos.x + "_" + pos.y + "_" + pos.z;
    }

    public int GetChunkSize()
    {
        return chunkSize;
    }

    IEnumerator BuildWorldHeight()
    {
        int playerPosX = (int)(player.transform.position.x / chunkSize);
        int playerPosZ = (int)(player.transform.position.z / chunkSize);

        for (int z = 0; z < worldRadius; z++)
        {
            for (int x = 0; x < worldRadius; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3((x + playerPosX) * chunkSize, y * chunkSize, (z + playerPosZ) * chunkSize);
                    string chunkName = SetChunkNameByPos(chunkPosition);
                    Chunk chunk = new Chunk(chunkPosition, matAtlas);
                    chunk.SpawnedChunk.transform.parent = this.transform;
                    AllChunks.Add(chunkName, chunk);
                    yield return null;
                }
            }
        }

        foreach (var chunk in AllChunks)
        {
            chunk.Value.DrawChunk();
            yield return null;
        }

        player.ActivatePlayer();
    }
}