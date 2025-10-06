using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] private MoveCamera moveCamera;

    [Header("UI")]
    [SerializeField] private GameObject itemNamedescriptionPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    [Header("Settings")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private AudioClip letterSound;
    [SerializeField] private float letterDelay = 0.03f;
    [SerializeField] private AudioSource audioSource;
    private List<ItemSlot> items = new List<ItemSlot>();
    private int selectedIndex = -1;
    private Coroutine typingCoroutine;
    private bool lastCameraState = true;
    // Start is called before the first frame update
    void Start()
    {
        if (moveCamera == null)
        {
            moveCamera = FindObjectOfType<MoveCamera>();
        }
        RefreshItemsList();
        HidePanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveCamera == null) return;
        if (lastCameraState != moveCamera.canMove)
        {
            lastCameraState = moveCamera.canMove;
            if (moveCamera.canMove)
            {
                HidePanel();
            }
            else
            {
                if (selectedIndex >= 0 && selectedIndex < items.Count)
                    ShowPanel();
            }
        }

        if (moveCamera.canMove) return;
        if (items.Count == 0) return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            SelectPrevious();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SelectNext();
        }
    }

    public void RefreshItemsList()
    {
        items.Clear();
        foreach (Transform child in itemContainer)
        {
            ItemSlot slot = child.GetComponent<ItemSlot>();
            if (slot != null)
                items.Add(slot);

        }
        if (selectedIndex >= items.Count)
            selectedIndex = items.Count - 1;
    }

    public void SelectPrevious()
    {
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = items.Count - 1;
        UpdateSelection();
    }

    public void SelectNext()
    {
        selectedIndex++;
        if (selectedIndex >= items.Count)
            selectedIndex = 0;
        UpdateSelection();
    }


    public void UpdateSelection()
    {
        for (int i = 0; i < items.Count; i++)
        {
            bool isSelected = (i == selectedIndex);
            items[i].SetSelected(isSelected);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count)
        {
            itemNamedescriptionPanel.SetActive(false);
            return;
        }
        
        ItemSlot current = items[selectedIndex];
        itemNameText.text = current.ItemName;
        ShowPanel();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(current.itemDescription));
    }

    private IEnumerator TypeText(string text)
    {
        itemDescriptionText.text = "";

        foreach (char c in text)
        {
            itemDescriptionText.text += c;

            if (letterSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(letterSound);
            }

            yield return new WaitForSeconds(letterDelay);
        }
    }

    private void ShowPanel()
    {
        itemNamedescriptionPanel.SetActive(true);
        itemNameText.gameObject.SetActive(true);
    }

    private void HidePanel()
    {
        itemNamedescriptionPanel.SetActive(false);
        itemNameText.gameObject.SetActive(false);
    }    

}
