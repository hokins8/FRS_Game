using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool building = false;

    private List<string> chunkToRemove = new List<string>();

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
        building = true;
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

            if (chunk.Value.SpawnedChunk != null)
            {
                //if (Vector3.Distance(player.transform.position, chunk.Value.SpawnedChunk.transform.position) > worldRadius * chunkSize * 2) // WIP
                //{
                //    chunkToRemove.Add(chunk.Key);
                //}
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

        if (firstBuild)
        {
            player.ActivatePlayer();
            if (loading != null)
                MainMenu_UI.Instance.UnloadLoadingScene();
            firstBuild = false;
        }
        building = false;
    }

    private IEnumerator RemoveOldChunk()
    {
        foreach (var chunkName in chunkToRemove.ToList())
        {
            if (AllChunks.ContainsKey(chunkName))
            {
                Destroy(AllChunks[chunkName].SpawnedChunk);
                AllChunks.Remove(chunkName);
                yield return null;
            }
        }
    }

    private void Update()
    {
        //if (!firstBuild && !building)
        //{
        //    StartCoroutine(BuildWorldHeight());
        //}
        if (!firstBuild)
        {
            StartCoroutine(BuildWorldHeight());
        }
        if (chunkToRemove.Count > 0)
        {
            StartCoroutine(RemoveOldChunk());
        }
    }
}