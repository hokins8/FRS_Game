using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

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
                    Chunk chunk = new Chunk(chunkPosition, matAtlas);
                    chunk.SpawnedChunk.transform.parent = this.transform;
                    AllChunks.Add(chunkName, chunk);

                    process++;
                    if (loading != null)
                    {
                        float value = process / totalChunks * 100;
                        loading.SetBar(value);
                    }

                    yield return null;
                }
            }
        }

        foreach (var chunk in AllChunks)
        {
            chunk.Value.DrawChunk();
            process++;
            if (loading != null)
            {
                float value = process / totalChunks * 100;
                loading.SetBar(value);
            }
            yield return null;
        }

        player.ActivatePlayer();

        if (loading != null)
            MainMenu_UI.Instance.UnloadLoadingScene();
    }
}