using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/BlockType", order = 1)]
public class BlockScriptObj : ScriptableObject
{
    [SerializeField] BlockType blockType;
    [SerializeField] Material blockMaterial;
    [SerializeField] float hardeness;
    [SerializeField] int posY;

    public BlockType GetBlockType()
    {
        return blockType;
    }

    public Material GetMaterial()
    {
        return blockMaterial;
    }

    public float GetHardeness()
    {
        return hardeness;
    }

    public int GetPosY()
    {
        return posY;
    }
}
