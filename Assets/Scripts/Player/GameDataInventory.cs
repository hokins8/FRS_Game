using System;
using System.Collections.Generic;

[Serializable]
public class GameDataInventory
{
    public Dictionary<BlockType, int> Inventory = new Dictionary<BlockType, int>();
}
