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
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCamera == null || outline == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RaycastRange))
        {
            var target = hit.collider.GetComponentInParent<CanUseOutliner>();
            if (target == this)
            {
                outline.enabled = true;
                Debug.Log("Camera hit");
                return;
            }
        }

        //If not just disable
        outline.enabled = false;
    }
}
