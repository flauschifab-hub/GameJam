//Fabio

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanUseOutliner : MonoBehaviour
{

    [Header("Outline Script")]
    public Outline outline;


    [Header("Settings")]
    public float RaycastRange = 5f;
    public string playerCameraTag = "MainCamera";

    private Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        //Find Cam
        GameObject camObj = GameObject.FindGameObjectWithTag(playerCameraTag);
        if (camObj != null)
            playerCamera = camObj.GetComponent<Camera>();

        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;

        Debug.Log("camera found");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCamera == null || outline == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RaycastRange))
        {
            if (hit.collider.gameObject == gameObject)
            {
                outline.enabled = true;
                Debug.Log("Camera hit");
            }
             return;
        }

        //If not just disable
        outline.enabled = false;
    }
}
