using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject cam;

    private int chunkSize;

    void Start()
    {
        chunkSize = World.Instance.GetChunkSize();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryDestroy();
    }

    private void TryDestroy()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4))
        {
            Vector3 hitBlock = hit.point - hit.normal / 2.0f;

            int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
            int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
            int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

            if (World.Instance.AllChunks.TryGetValue(hit.collider.gameObject.name, out Chunk hitChunk))
            {
                hitChunk.GetChunkData()[x, y, z].HitBlock();

                List<string> neighboursUpdates = new List<string>();
                float thisChunkx = hitChunk.SpawnedChunk.transform.position.x;
                float thisChunky = hitChunk.SpawnedChunk.transform.position.y;
                float thisChunkz = hitChunk.SpawnedChunk.transform.position.z;

                if (x == 0)
                    neighboursUpdates.Add(World.Instance.SetChunkNameByPos(new Vector3(thisChunkx - chunkSize, thisChunky, thisChunkz)));
                if (x == chunkSize - 1)
                    neighboursUpdates.Add(World.Instance.SetChunkNameByPos(new Vector3(thisChunkx + chunkSize, thisChunky, thisChunkz)));
                if (y == 0)
                    neighboursUpdates.Add(World.Instance.SetChunkNameByPos(new Vector3(thisChunkx, thisChunky - chunkSize, thisChunkz)));
                if (y == chunkSize - 1)
                    neighboursUpdates.Add(World.Instance.SetChunkNameByPos(new Vector3(thisChunkx, thisChunky + chunkSize, thisChunkz)));
                if (z == 0)
                    neighboursUpdates.Add(World.Instance.SetChunkNameByPos(new Vector3(thisChunkx, thisChunky, thisChunkz - chunkSize)));
                if (z == chunkSize - 1)
                    neighboursUpdates.Add(World.Instance.SetChunkNameByPos(new Vector3(thisChunkx, thisChunky, thisChunkz + chunkSize)));

                foreach (string cname in neighboursUpdates)
                {
                    if (World.Instance.AllChunks.TryGetValue(cname, out Chunk chunk))
                    {
                        chunk.Redraw();
                    }
                }
            }
        }
    }
}
