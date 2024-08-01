using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    // WIP

    void Start()
    {
        playerObject.SetActive(false);
    }

    public void ActivatePlayer()
    {
        playerObject.SetActive(true);
    }
}
