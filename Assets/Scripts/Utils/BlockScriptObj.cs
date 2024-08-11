using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/BlockType", order = 1)]
public class BlockScriptObj : ScriptableObject
{
    [SerializeField] BlockType blockType;
    [SerializeField] Sprite blockSprite;
    [SerializeField] Material blockMaterial; // old
    [SerializeField] int hardeness;

    public BlockType GetBlockType()
    {
        return blockType;
    }

    public Material GetMaterial()
    {
        return blockMaterial;
    }

    public Sprite GetBlockSprite()
    {
        return blockSprite;
    }

    public int GetHardeness()
    {
        return hardeness;
    }
}
