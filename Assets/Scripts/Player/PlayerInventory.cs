using System.Collections;
using System.Collections.Generic;
using Tools.UnityUtilities;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System;
using System.IO;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] PlayerInventoryElementUI inventoryElementUI_prefab;
    [SerializeField] Transform inventoryRoot;

    [SerializeField] TMP_Text currentBlockTypeText;

    private GameDataInventory gameDataInventory;

    private BlockType blockTypeToSpawn = BlockType.Grass;

    private List<PlayerInventoryElementUI> activeInventoryElement = new();

    private readonly Pool<PlayerInventoryElementUI> inventoryElementPool = new()
    {
        AutoExpand = true,
        ExpansionStep = 10,
    };

    void Awake()
    {
        LoadInventory();
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

    private string GetInventoryPath()
    {
        var mainMenu = MainMenu_UI.Instance;
        string folderPath = $"{mainMenu.SavesLocation}\\{mainMenu.CurrentFolder}/Inventory";
        string fileName = "Inventory.json";
        string fullPath = Path.Combine(folderPath, fileName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return fullPath;
    }

    public void SaveInventory()
    {
        string json = JsonConvert.SerializeObject(gameDataInventory, Formatting.Indented);
        File.WriteAllText(GetInventoryPath(), json);
    }

    private void LoadInventory()
    {
        var path = GetInventoryPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameDataInventory = JsonConvert.DeserializeObject<GameDataInventory>(json);
            foreach (var item in gameDataInventory.Inventory)
                Debug.Log($"{item.Key}: {item.Value}");
        }
        else
        {
            gameDataInventory = new GameDataInventory();
            gameDataInventory.Inventory.Add(BlockType.Snow, 5);
            gameDataInventory.Inventory.Add(BlockType.Grass, 5);
            gameDataInventory.Inventory.Add(BlockType.Rock, 5);
        }
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
            if (gameDataInventory.Inventory.ContainsKey(blockType))
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
                element.UpdateCount(gameDataInventory.Inventory[element.BlockType]);
    }

    public BlockType GetCurrentBlockTypeToSpawn()
    {
        return blockTypeToSpawn;
    }

    public Dictionary<BlockType, int> GetPlayerInventory()
    {
        return gameDataInventory.Inventory;
    }

    public void UseInventory(BlockType type, bool remove = false)
    {
        if (gameDataInventory.Inventory.ContainsKey(type))
        {
            if (remove)
            {
                if (gameDataInventory.Inventory[type] > 0)
                    gameDataInventory.Inventory[type]--;
            }
            else
            {
                gameDataInventory.Inventory[type]++;
            }
        }
        else
        {
            Debug.LogError("Missing BlockType key in inventory " + type);
        }
    }
}
