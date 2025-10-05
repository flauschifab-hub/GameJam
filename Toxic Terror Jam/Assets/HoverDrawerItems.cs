using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverDrawerItems : MonoBehaviour
{
    [Header("Settings")]

    public Camera playerCamera;

    public float RaycastRange = 5f;
    public Outline Outline;
    public float ScaleSpeed;
    public TMP_Text HoverText;
    public string fullText = "Test";
    public float ScaleUpAmount = 1.2f;
    public float TypingSpeed = 0.05f;

    public AudioSource AudioSource;
    public AudioClip TypeSFX;
    private Vector3 originalScale;
    private Coroutine TypingCoroutine;
    private bool isHovered;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        if (Outline != null)
            Outline.enabled = false;

        if (HoverText != null)
            HoverText.text = "";
    }

    void Update()
    {
        CheckHover();
    }

void CheckHover()
{
    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
    Debug.DrawRay(ray.origin, ray.direction * RaycastRange, Color.green);

    RaycastHit[] hits = Physics.RaycastAll(ray, RaycastRange);
    RaycastHit? closestValidHit = null;
    float closestDistance = Mathf.Infinity;

    foreach (RaycastHit hit in hits)
    {
        if (hit.collider.CompareTag("DrawerItems"))
        {
            if (hit.distance < closestDistance)
            {
                closestValidHit = hit;
                closestDistance = hit.distance;
            }
        }
    }

    bool hitThis = false;

    if (closestValidHit.HasValue)
    {
        if (closestValidHit.Value.collider.GetComponentInParent<HoverDrawerItems>() == this)
            hitThis = true;
    }

    if (hitThis && !isHovered)
        OnHoverEnter();
    else if (!hitThis && isHovered)
        OnHoverExit();
}



    

    private void OnHoverEnter()
    {
        isHovered = true;
        if (Outline != null)
            Outline.enabled = true;

        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * ScaleUpAmount));

        if (HoverText != null)
        {
            if (TypingCoroutine != null)
                StopCoroutine(TypingCoroutine);

            TypingCoroutine = StartCoroutine(TypeText());
        }
    }


    private void OnHoverExit()
    {
        isHovered = false;
        if (Outline != null)
            Outline.enabled = false;

        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));

        if (HoverText != null)
        {
            if (TypingCoroutine != null)
                StopCoroutine(TypingCoroutine);

            HoverText.text = "";
        }
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * ScaleSpeed);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator TypeText()
    {
        HoverText.text = "";
        foreach (char c in fullText)
        {
            HoverText.text += c;
            if (TypeSFX != null && AudioSource != null)
            {
                AudioSource.pitch = Random.Range(0.95f, 1.05f);
                AudioSource.PlayOneShot(TypeSFX);
            }

            yield return new WaitForSeconds(TypingSpeed);
        }
    }
}
