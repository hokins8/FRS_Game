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
        if (Input.GetMouseButtonDown(1))
            TrySpawnBlock();
    }

    private void TryDestroy()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4))
        {
            World world = World.Instance;
            if (!world.AllChunks.TryGetValue(hit.collider.gameObject.name, out _))
                return;

            Vector3 hitBlock = hit.point - hit.normal / 2.0f;

            Block block = world.GetWorldBlock(hitBlock);
            if (block == null)
                return;
            Chunk hitChunk = block.GetChunkParent();

            block.HitBlock();

            List<string> neighboursUpdates = new List<string>();
            float chunkX = hitChunk.SpawnedChunk.transform.position.x;
            float chunkY = hitChunk.SpawnedChunk.transform.position.y;
            float chunkZ = hitChunk.SpawnedChunk.transform.position.z;

            if (block.GetPosition().x == 0)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX - chunkSize, chunkY, chunkZ)));
            if (block.GetPosition().x == chunkSize - 1)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX + chunkSize, chunkY, chunkZ)));
            if (block.GetPosition().y == 0)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY - chunkSize, chunkZ)));
            if (block.GetPosition().y == chunkSize - 1)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY + chunkSize, chunkZ)));
            if (block.GetPosition().z == 0)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY, chunkZ - chunkSize)));
            if (block.GetPosition().z == chunkSize - 1)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY, chunkZ + chunkSize)));

            foreach (string cname in neighboursUpdates)
                if (world.AllChunks.TryGetValue(cname, out Chunk chunk))
                    chunk.Redraw();
        }
    }

    private void TrySpawnBlock()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4))
        {
            World world = World.Instance;
            if (!world.AllChunks.TryGetValue(hit.collider.gameObject.name, out _))
                return;

            Vector3 hitBlock = hit.point + hit.normal / 2.0f;

            Block block = world.GetWorldBlock(hitBlock);
            if (block == null) 
                return;
            Chunk hitChunk = block.GetChunkParent();

            block.BuildBlock(BlockType.Grass); // WIP + add CubeWire

            List<string> neighboursUpdates = new List<string>();
            float chunkX = hitChunk.SpawnedChunk.transform.position.x;
            float chunkY = hitChunk.SpawnedChunk.transform.position.y;
            float chunkZ = hitChunk.SpawnedChunk.transform.position.z;

            if (block.GetPosition().x == 0)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX - chunkSize, chunkY, chunkZ)));
            if (block.GetPosition().x == chunkSize - 1)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX + chunkSize, chunkY, chunkZ)));
            if (block.GetPosition().y == 0)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY - chunkSize, chunkZ)));
            if (block.GetPosition().y == chunkSize - 1)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY + chunkSize, chunkZ)));
            if (block.GetPosition().z == 0)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY, chunkZ - chunkSize)));
            if (block.GetPosition().z == chunkSize - 1)
                neighboursUpdates.Add(world.SetChunkNameByPos(new Vector3(chunkX, chunkY, chunkZ + chunkSize)));

            foreach (string cname in neighboursUpdates)
                if (world.AllChunks.TryGetValue(cname, out Chunk chunk))
                    chunk.Redraw();
        }
    }
}
