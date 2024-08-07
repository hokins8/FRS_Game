using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    private bool firstBuild = true;

    private List<string> chunkToRemove = new List<string>();

    public static World Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Vector3 pos = player.transform.position;
        player.transform.position = new Vector3(pos.x, PerlinNoise.Instance.GenerateGrassHeight(pos.x, pos.z) + 1, pos.z);
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

    public int GetWorldRadius()
    {
        return worldRadius;
    }

    private async void TryBuildAsyncWorld(Vector3 playerPos)
    {
        await AsyncBuildWorld(playerPos);
    }

    private async Task AsyncBuildWorld(Vector3 playerPos)
    {
        await Task.Yield();

        int playerPosX = (int)(playerPos.x / chunkSize);
        int playerPosZ = (int)(playerPos.z / chunkSize);

        for (int z = 0; z < worldRadius; z++)
        {
            for (int x = 0; x < worldRadius; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3((x + playerPosX) * chunkSize, y * chunkSize, (z + playerPosZ) * chunkSize);
                    string chunkName = SetChunkNameByPos(chunkPosition);

                    if (chunkToRemove.Contains(chunkName) || Vector3.Distance(playerPos, chunkPosition) > worldRadius * chunkSize)
                        continue;

                    Chunk chunk;
                    if (!AllChunks.TryGetValue(chunkName, out _))
                    {
                        chunk = new Chunk(chunkPosition, matAtlas);
                        chunk.SpawnedChunk.transform.parent = this.transform;
                        AllChunks.Add(chunkName, chunk);
                    }
                    else
                    {
                        break;
                    }
                    await Task.Yield();
                }
            }
        }

        foreach (var chunk in AllChunks.ToList())
        {
            if (chunk.Value.ChunkStatus == ChunkStatus.ReadyToDraw)
            {
                chunk.Value.DrawChunk();
            }
            chunk.Value.ChunkStatus = ChunkStatus.Done;

            if (chunk.Value.SpawnedChunk != null)
            {
                if (Vector3.Distance(playerPos, chunk.Value.SpawnedChunk.transform.position) > worldRadius * chunkSize)
                {
                    chunkToRemove.Add(chunk.Key);
                }
            }
            await Task.Yield();
        }
    }

    private IEnumerator BuildWorldHeight()
    {
        int playerPosX = (int)(player.transform.position.x / chunkSize);
        int playerPosZ = (int)(player.transform.position.z / chunkSize);

        float totalChunks = worldRadius * worldRadius * worldHeight;
        float process = 0;

        LoadingController loading = LoadingController.Instance;

        for (int z = 0; z < worldRadius; z++)
        {
            for (int x = 0; x < worldRadius; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3((x + playerPosX) * chunkSize, y * chunkSize, (z + playerPosZ) * chunkSize);
                    string chunkName = SetChunkNameByPos(chunkPosition);

                    if (chunkToRemove.Contains(chunkName) || Vector3.Distance(player.transform.position, chunkPosition) > worldRadius * chunkSize)
                        continue;

                    Chunk chunk;
                    if (!AllChunks.TryGetValue(chunkName, out _))
                    {
                        chunk = new Chunk(chunkPosition, matAtlas);
                        chunk.SpawnedChunk.transform.parent = this.transform;
                        AllChunks.Add(chunkName, chunk);
                    }
                    else
                    {
                        break;
                    }

                    if (firstBuild)
                    {
                        process++;
                        if (loading != null)
                        {
                            float value = process / totalChunks * 100;
                            loading.SetBar(value);
                        }
                    }

                    yield return null;
                }
            }
        }

        foreach (var chunk in AllChunks.ToList())
        {
            if (chunk.Value.ChunkStatus == ChunkStatus.ReadyToDraw)
            {
                chunk.Value.DrawChunk();
            }
            chunk.Value.ChunkStatus = ChunkStatus.Done;

            if (firstBuild)
            {
                process++;
                if (loading != null)
                {
                    float value = process / totalChunks * 100;
                    loading.SetBar(value);
                }
            }
            
            yield return null;
        }

        if (firstBuild)
        {
            player.ActivatePlayer();
            if (loading != null)
                MainMenu_UI.Instance.UnloadLoadingScene();
            firstBuild = false;
        }
    }

    private IEnumerator RemoveOldChunk()
    {
        foreach (var chunkName in chunkToRemove.ToList())
        {
            if (AllChunks.ContainsKey(chunkName))
            {
                AllChunks[chunkName].Save(); // WIP -> Better save
                Destroy(AllChunks[chunkName].SpawnedChunk);
                AllChunks.Remove(chunkName);
                yield return null;
            }
        }
    }

    private void Update()
    {
        if (chunkToRemove.Count > 0)
            StartCoroutine(RemoveOldChunk());
        if (!firstBuild)
            TryBuildAsyncWorld(player.transform.position);
    }
}