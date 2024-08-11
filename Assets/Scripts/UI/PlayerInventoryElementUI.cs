using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryElementUI : MonoBehaviour
{
    public BlockType BlockType = BlockType.None;

    [SerializeField] Image blockTexture;
    [SerializeField] TMP_Text blockText;
    [SerializeField] TMP_Text blockCount;

    public void SetupInventoryElementUI(BlockType type)
    {
        var block = BlockCollection.Instance.GetBlock(type);
        if (block != null)
        {
            BlockType = type;
            blockTexture.sprite = block.GetBlockSprite();
            blockText.text = type.ToString();
            blockCount.text = Player.Instance.PlayerInventory.GetPlayerInventory()[type].ToString();
        }
    }

    public void UpdateCount(int value)
    {
        blockCount.text = value.ToString();
    }
}
