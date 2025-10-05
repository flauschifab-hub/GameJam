using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSelector : MonoBehaviour
{

    [Header("UI")]
    [SerializeField]private GameObject itemNamedescriptionPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField]private TextMeshProUGUI itemDescriptionText;

    [Header("Settings")]
    [SerializeField]private Transform itemContainer;
    private List<ItemSlot> items = new List<ItemSlot>();
    private int selectedIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        RefreshItemsList();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
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
            itemNameText.text = "";
            itemDescriptionText.text = "";
            return;
        }

        ItemSlot current = items[selectedIndex];
        itemNameText.text = current.ItemName;
        itemDescriptionText.text = current.itemDescription;
    }
    

}
