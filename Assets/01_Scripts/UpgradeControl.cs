using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeControl : MonoBehaviour
{
    public ItemSlot[] slots;
    public GameObject invWindow;
    public Transform slotPanel;

    [Header("Selected Item")]
    ItemSlot selectedItem;
    int selectedItemIdx;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public GameObject confirmButton;

    private void Start()
    {
        invWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].idx = i;
            slots[i].inv = this;

            ClearSelectedItemWindow();
        }
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
    }

    public void Toggle()
    {
        invWindow.SetActive(!IsOpen());
    }

    public bool IsOpen()
    {
        return invWindow.activeSelf;
    }

    public void SelectItem(int idx)
    {
        if (slots[idx].item == null) return;

        selectedItem = slots[idx];
        selectedItemIdx = idx;

        selectedItemName.text = selectedItem.item.Name;
        selectedItemDescription.text = selectedItem.item.Description;
    }

    public void AddItem(ItemData data)
    {
        if (data.canStack)
        {
            ItemSlot slot = GetItemSlot(data);
            if (slot != null)
            {
                slot.count += data.Count;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.count = 1;
            
            UpdateUI();
            return;
        }
    }

    ItemSlot GetItemSlot(ItemData data)
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].item == data)
            {
                return slots[i];
            }
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].item == null)
                return slots[i];
        }

        return null;
    }

    public void UpdateUI()
    {
        for(int i=0; i<slots.Length; ++i)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
        }
    }
}
