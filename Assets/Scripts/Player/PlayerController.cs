using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 2.0f;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(moveHorizontal, 0, moveVertical);

        float rotationHorizontal = Input.GetAxis("Mouse X") * sensitivity;
        float rotationVertical = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(0, rotationHorizontal, 0);
        Camera.main.transform.Rotate(-rotationVertical, 0, 0);
    }
}
