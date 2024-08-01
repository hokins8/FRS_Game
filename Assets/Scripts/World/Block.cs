using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockSide
{
    None,
    Down,
    Up,
    Right,
    Left,
    Back,
    Front,
};

public enum BlockType
{
    None,
    Rock,
    Grass,
    Snow,
    Floor,
}

public class Block : MonoBehaviour
{
    private BlockType blockType;
    private Chunk chunkParent;
    private Vector3 position;
    private bool haveNeighbor;

    private readonly Vector2[,] blockUVs = 
    {
        // Grass
        {new Vector2(0, 0.5f), new Vector2(0.0625f, 0.5f), new Vector2(0, 0.5625f), new Vector2(0.0625f, 0.5625f)},
        // Snow
        {new Vector2(0.5f, 0.5f), new Vector2(0.5625f, 0.5f), new Vector2(0.5f, 5625f), new Vector2(0.5625f, 0.5625f)},
        // Rock
        {new Vector2(0.25f, 0.5f), new Vector2(0.3125f, 0.5f), new Vector2(0.25f, 0.5625f), new Vector2(0.3125f, 0.5625f)},
        // Floor
        {new Vector2(0.75f, 0.25f), new Vector2(0.8125f, 0.25f), new Vector2(0.75f, 0.3125f), new Vector2(0.8125f, 0.3125f)}
    };

    public Block(BlockType type, Vector3 pos, Chunk chunk)
    {
        blockType = type;
        position = pos;
        chunkParent = chunk;
        haveNeighbor = type != BlockType.None;
    }

    private void CreateQuad(BlockSide side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];

        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        Vector2 uv = blockUVs[0, 0];

        switch (blockType)
        {
            case BlockType.None:
                break;
            case BlockType.Rock:
                uv = blockUVs[2, 2];
                break;
            case BlockType.Grass:
                uv = blockUVs[0, 0];
                break;
            case BlockType.Snow:
                uv = blockUVs[1, 1];
                break;
            case BlockType.Floor:
                uv = blockUVs[3, 3];
                break;
            default:
                uv = blockUVs[0, 0];
                break;
        }

        switch (side)
        {
            case BlockSide.Down:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down};
                break;
            case BlockSide.Up:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
                break;
            case BlockSide.Right:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right};
                break;
            case BlockSide.Left:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left};
                break;
            case BlockSide.Front:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
                break;
            case BlockSide.Back:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back};
                break;
            default:
                break;
        }
        Vector2[] uvs = new Vector2[] { uv, uv, uv, uv };
        int[] triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        quad.transform.position = position;
        quad.transform.parent = chunkParent.SpawnedChunk.transform;

        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private int ConvertBlockIndexToLocal(int pos, int chunkSize)
    {
        if (pos == -1)
            pos = chunkSize - 1;
        else if (pos == chunkSize)
            pos = 0;

        return pos;
    }

    public bool HasNeighbour(int x, int y, int z)
    {
        Block[,,] chunks;

        var chunkSize = World.Instance.ChunkSize;
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize)
        {
            Vector3 neighbourChunksPos = chunkParent.SpawnedChunk.transform.position + new Vector3((x - (int)position.x) * chunkSize, (y - (int)position.y) * chunkSize, (z - (int)position.z) * chunkSize);
            x = ConvertBlockIndexToLocal(x, chunkSize);
            y = ConvertBlockIndexToLocal(y, chunkSize);
            z = ConvertBlockIndexToLocal(z, chunkSize);

            string chunkName = World.Instance.SetChunkNameByPos(neighbourChunksPos);

            Chunk nextChunk;
            if (World.Instance.AllChunks.TryGetValue(chunkName, out nextChunk))
                chunks = nextChunk.GetChunkData();
            else
                return false;
        }
        else
        {
            chunks = chunkParent.GetChunkData();
        }


        try
        {
            return chunks[x, y, z].haveNeighbor;
        }
        catch (System.Exception) 
        {
            return false;
        }
    }


    public void Draw()
    {
        if (blockType == BlockType.None)
            return;

        TryDraw(BlockSide.Front, (int)position.x, (int)position.y, (int)position.z + 1);
        TryDraw(BlockSide.Back, (int)position.x, (int)position.y, (int)position.z - 1);
        TryDraw(BlockSide.Down, (int)position.x, (int)position.y - 1, (int)position.z);
        TryDraw(BlockSide.Up, (int)position.x, (int)position.y + 1, (int)position.z);
        TryDraw(BlockSide.Right, (int)position.x + 1, (int)position.y, (int)position.z);
        TryDraw(BlockSide.Left, (int)position.x - 1, (int)position.y, (int)position.z);
    }

    private void TryDraw(BlockSide side, int x, int y, int z)
    {
        if (!HasNeighbour(x, y, z))
            CreateQuad(side);
    }
}