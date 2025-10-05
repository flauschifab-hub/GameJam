using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    [Header("TMProText")]
    public string ItemName;
    [TextArea]public string itemDescription;

    private Outline outline;
    private Vector3 originalScale;
    private bool initialized = false;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        originalScale = transform.localScale;
        initialized = true;
    }

    public void SetSelected(bool selected)
    {
        if (!initialized)
        {
            originalScale = transform.localScale;
            initialized = true;
        }
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = selected;

        transform.localScale = selected ? originalScale * 1.15f : originalScale;
    }
}
