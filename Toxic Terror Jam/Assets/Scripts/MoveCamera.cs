using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code von Jan

public class MoveCamera : MonoBehaviour
{

    [Header("Settings")]
    public float sensitivity = 200f;

    public float minXRotation;
    public float maxXRotation;

    public float minYRotation;
    public float maxYRotation;
    float xRotation = 0f; //Pitch
    float yRotation = 0f; //Yaw

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void Update()
    {
        //mäuschenkontrolle
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;


        xRotation -= mouseY;//hoch und drunter  movement
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        yRotation += mouseX;//das gleich aber links und rechts
        yRotation = Mathf.Clamp(yRotation, minYRotation, maxYRotation);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
