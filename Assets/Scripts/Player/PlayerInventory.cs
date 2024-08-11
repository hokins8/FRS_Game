using System.Collections;
using System.Collections.Generic;
using Tools.UnityUtilities;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] PlayerInventoryElementUI inventoryElementUI_prefab;
    [SerializeField] Transform inventoryRoot;

    [SerializeField] TMP_Text currentBlockTypeText;

    private Dictionary<BlockType, int> inventory = new Dictionary<BlockType, int>(); // WIP serialize

    private BlockType blockTypeToSpawn = BlockType.Grass;

    private List<PlayerInventoryElementUI> activeInventoryElement = new();

    private readonly Pool<PlayerInventoryElementUI> inventoryElementPool = new()
    {
        AutoExpand = true,
        ExpansionStep = 10,
    };

    void Awake()
    {
        inventory.Add(BlockType.Snow, 5);
        inventory.Add(BlockType.Grass, 5);
        inventory.Add(BlockType.Rock, 5);

        CreatePool();
    }

    void Start()
    {
        SpawnInventoryElement();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            SetCurrentBlockType(BlockType.Grass);
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            SetCurrentBlockType(BlockType.Rock);
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
            SetCurrentBlockType(BlockType.Snow);
    }

    private void SetCurrentBlockType(BlockType type)
    {
        blockTypeToSpawn = type;
        currentBlockTypeText.text = "Current Type: \n" + blockTypeToSpawn.ToString();
    }

    private void CreatePool()
    {
        inventoryElementPool.SetFactory(() => Instantiate(inventoryElementUI_prefab, inventoryRoot));
        inventoryElementPool.OnFree = i => { i.gameObject.SetActive(false); };
        inventoryElementPool.OnUse = i => { i.gameObject.SetActive(true); };
        inventoryElementPool.CreateElements(10);
    }

    public void SpawnInventoryElement()
    {
        inventoryElementPool.ReturnAll();
        activeInventoryElement.Clear();

        var blockList = BlockCollection.Instance.BlockScriptObjs;

        foreach (var block in blockList)
        {
            BlockType blockType = block.GetBlockType();
            if (inventory.ContainsKey(blockType))
            {
                inventoryElementPool.GetElement(out var element);
                element.SetupInventoryElementUI(blockType);
                element.transform.SetAsLastSibling();
                activeInventoryElement.Add(element);
            }
        }
    }

    public void UpdateInventory()
    {
        foreach (var element in activeInventoryElement)
            if (element.BlockType != BlockType.None)
                element.UpdateCount(inventory[element.BlockType]);
    }

    public BlockType GetCurrentBlockTypeToSpawn()
    {
        return blockTypeToSpawn;
    }

    public Dictionary<BlockType, int> GetPlayerInventory()
    {
        return inventory;
    }

    public void UseInventory(BlockType type, bool remove = false)
    {
        if (inventory.ContainsKey(type))
        {
            if (remove)
            {
                if (inventory[type] > 0)
                    inventory[type]--;
            }
            else
            {
                inventory[type]++;
            }
        }
        else
        {
            Debug.LogError("Missing BlockType key in inventory " + type);
        }
    }
}
