using System.Collections;
using UnityEngine;
using TMPro;

public class HoverDrawerItems : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;         // Assign the active camera (DrawerAnim.TargetCamera)
    public MoveCamera MoveCameraScript; // Reference to MoveCamera
    public Outline Outline;             // Optional outline component
    public TMP_Text HoverText;          // Optional hover text

    [Header("Settings")]
    public float RaycastRange = 10f;    // Max distance for ray
    public float ScaleSpeed = 10f;
    public float ScaleUpAmount = 1.2f;
    public float TypingSpeed = 0.05f;
    [TextArea] public string fullText = "Test"; // Text to display on hover

    [Header("Audio")]
    public AudioSource AudioSource;     // Optional AudioSource
    public AudioClip TypeSFX;           // Optional typing sound

    private Vector3 originalScale;
    private Coroutine TypingCoroutine;
    private Coroutine ScaleCoroutine;
    private bool isHovered;

    void Start()
    {
        originalScale = transform.localScale;

        if (Outline != null)
            Outline.enabled = false;

        if (HoverText != null)
            HoverText.text = "";

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        CheckHover();
    }

    void CheckHover()
    {
        Ray ray;

        // Determine ray source: mouse or camera center
        if (Cursor.visible && Cursor.lockState != CursorLockMode.Locked)
        {
            ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }

        // Fix: Correct ray direction when MoveCamera is disabled
        if (MoveCameraScript != null && !MoveCameraScript.canMove)
        {
            ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        }

        Debug.DrawRay(ray.origin, ray.direction * RaycastRange, Color.green);

        // Simplified: Only check closest hit
        if (Physics.Raycast(ray, out RaycastHit hit, RaycastRange))
        {
            var item = hit.collider.GetComponentInParent<HoverDrawerItems>();
            if (item == this)
            {
                if (!isHovered)
                    OnHoverEnter();
                return;
            }
        }

        // No valid hit on this object
        if (isHovered)
            OnHoverExit();
    }

    void OnHoverEnter()
    {
        isHovered = true;

        if (Outline != null)
            Outline.enabled = true;

        if (ScaleCoroutine != null)
            StopCoroutine(ScaleCoroutine);
        ScaleCoroutine = StartCoroutine(ScaleTo(originalScale * ScaleUpAmount));

        if (HoverText != null)
        {
            if (TypingCoroutine != null)
                StopCoroutine(TypingCoroutine);

            TypingCoroutine = StartCoroutine(TypeText());
        }
    }

    void OnHoverExit()
    {
        isHovered = false;

        if (Outline != null)
            Outline.enabled = false;

        if (ScaleCoroutine != null)
            StopCoroutine(ScaleCoroutine);
        ScaleCoroutine = StartCoroutine(ScaleTo(originalScale));

        if (HoverText != null)
        {
            if (TypingCoroutine != null)
                StopCoroutine(TypingCoroutine);

            HoverText.text = "";
        }
    }

    IEnumerator ScaleTo(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * ScaleSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    IEnumerator TypeText()
    {
        HoverText.text = "";
        foreach (char c in fullText)
        {
            HoverText.text += c;

            if (TypeSFX != null && AudioSource != null)
            {
                if (Random.value > 0.5f) // reduces sound spam
                {
                    AudioSource.pitch = Random.Range(0.95f, 1.05f);
                    AudioSource.PlayOneShot(TypeSFX);
                }
            }

            yield return new WaitForSeconds(TypingSpeed);
        }
    }
}
