//Fabio
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerAnim : MonoBehaviour
{

    [Header("Generall")]
    public Animator animator;
    public string PullingOutTrigger = "PullingOut";
    public string PushingInTrigger = "PushingIn";
    public float RaycastRange = 5f;

    [Header("Camera")]

    public Transform TargetCamera;
    public Transform OriginalCameraPosition;

    public Transform drawerLookAtPos;
    public float CameraMoveSpeed = 5f;

    public MoveCamera MoveCameraScript;

    private bool isPulledOut = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera camComponent = TargetCamera.GetComponent<Camera>();
            if (camComponent == null) return;

            Ray ray = new Ray(TargetCamera.transform.position, TargetCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RaycastRange))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (!isPulledOut)
                    {
                        if (MoveCameraScript != null)
                        {
                            MoveCameraScript.canMove = false;
                        }
                        Debug.Log("MoveCamera Disabled");
                        animator.SetTrigger(PullingOutTrigger);
                        StopAllCoroutines();
                        StartCoroutine(MoveCameraTo(TargetCamera, drawerLookAtPos.position, drawerLookAtPos.rotation, false));
                        isPulledOut = true;
                    }
                    else
                    {
                        animator.SetTrigger(PushingInTrigger);
                        StopAllCoroutines();
                        StartCoroutine(MoveCameraTo(TargetCamera, OriginalCameraPosition.position, OriginalCameraPosition.rotation, true));
                        isPulledOut = false;
                    }
                }
            }
        }
    }

    System.Collections.IEnumerator MoveCameraTo(Transform cam, Vector3 targetPos, Quaternion targetRot, bool reEnableScript)
    {
        while (Vector3.Distance(cam.position, targetPos) > 0.01f || Quaternion.Angle(cam.rotation, targetRot) > 0.1f)
        {
            cam.position = Vector3.Lerp(cam.position, targetPos, Time.deltaTime * CameraMoveSpeed);
            cam.rotation = Quaternion.Lerp(cam.rotation, targetRot, Time.deltaTime * CameraMoveSpeed);
            yield return null;
        }
        cam.position = targetPos;
        cam.rotation = targetRot;

        if (reEnableScript && MoveCameraScript != null)
            MoveCameraScript.canMove = true;
             MoveCameraScript.SetRotation(cam.rotation);

    }
}
