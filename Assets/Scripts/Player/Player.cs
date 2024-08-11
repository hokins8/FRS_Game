using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInventory PlayerInventory;
    [SerializeField] GameObject playerObject;

    public static Player Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerObject.SetActive(false);
    }

    public void ActivatePlayer()
    {
        playerObject.SetActive(true);
    }
}
