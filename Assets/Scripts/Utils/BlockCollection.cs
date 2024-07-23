using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockCollection : MonoBehaviour
{
    public static BlockCollection Instance;

    [HideInInspector] public List<BlockScriptObj> BlockScriptObjs = new List<BlockScriptObj>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BlockScriptObjs = Resources.LoadAll<BlockScriptObj>("BlockTypes").ToList();

        foreach (var obj in BlockScriptObjs)
        {
            if (obj.GetBlockType() == BlockType.None)
                Debug.LogError("Bad Set of BlockType");
            if (obj.GetMaterial() == null)
                Debug.LogError("Null Material");
        }
    }
}
