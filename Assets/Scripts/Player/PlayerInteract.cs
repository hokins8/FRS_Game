using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject blockWire;

    private int chunkSize;

    private bool showWire = false;

    void Start()
    {
        chunkSize = World.Instance.GetChunkSize();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // WIP -> possibly rework to the new input system
            TryDestroy();

        if (Input.GetMouseButtonDown(1) && showWire)
            TrySpawnBlock();

        if (Input.GetKey(KeyCode.LeftShift))
            ShowBlockWire();

        if (Input.GetKeyUp(KeyCode.LeftShift))
            TurnOffWire();
    }

    private void TurnOffWire()
    {
        showWire = false;
        blockWire.SetActive(showWire);
    }

    private void ShowBlockWire()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 4))
        {
            World world = World.Instance;
            if (!world.AllChunks.TryGetValue(hit.collider.gameObject.name, out _))
                return;

            Vector3 hitBlock = hit.point + hit.normal / 2f;

            Block block = world.GetWorldBlock(hitBlock);

            if (block == null)
                return;

            Chunk hitChunk = block.GetChunkParent();

            Vector3 chunkPos = hitChunk.SpawnedChunk.transform.position;
            Vector3 blockPos = block.GetPosition();
            Vector3 blockPosition = chunkPos + blockPos;

            blockWire.transform.position = blockPosition;
            showWire = true;
            blockWire.SetActive(showWire);
        }
    }

    private void TryDestroy()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 4))
        {
            World world = World.Instance;
            if (!world.AllChunks.TryGetValue(hit.collider.gameObject.name, out _))
                return;

            Vector3 hitBlock = hit.point - hit.normal / 2f;

            Block block = world.GetWorldBlock(hitBlock);

            if (block == null)
                return;

            if (block.GetBlockType() == BlockType.Floor)
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

            foreach (string chunkName in neighboursUpdates)
                if (world.AllChunks.TryGetValue(chunkName, out Chunk chunk))
                    chunk.Redraw();
        }
    }

    private void TrySpawnBlock()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 4))
        {
            World world = World.Instance;
            if (!world.AllChunks.TryGetValue(hit.collider.gameObject.name, out _))
                return;

            Vector3 hitBlock = hit.point + hit.normal / 2f;

            Block block = world.GetWorldBlock(hitBlock);
            if (block == null) 
                return;
            Chunk hitChunk = block.GetChunkParent();

            block.BuildBlock(BlockType.Grass); // WIP

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

            TurnOffWire();
        }
    }
}
