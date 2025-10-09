using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class AchievementsHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Assignables")]
    [SerializeField] private RectTransform panelToFold;
    [SerializeField] private float foldSpeed = 5f;
    [SerializeField] private Vector2 foldedSize = new Vector2(0f, 50f);
    [SerializeField] private Vector2 unfoaldSize = new Vector2(200f, 50f);

    private bool isHovered = false;
    private Vector2 targetSize;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        if (panelToFold != null)
        {
            panelToFold.sizeDelta = foldedSize;
            canvasGroup = panelToFold.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panelToFold.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            targetSize = foldedSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (panelToFold == null) return;
        panelToFold.sizeDelta = Vector2.Lerp(panelToFold.sizeDelta, targetSize, Time.deltaTime * foldSpeed);

        float targetAlpha = isHovered ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * foldSpeed);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetSize = unfoaldSize;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetSize = foldedSize;

        StartCoroutine(DissableAfterHidden());
    }

    private System.Collections.IEnumerator DissableAfterHidden()
    {
        yield return new WaitForSeconds(0.2f);
        if (!isHovered)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
