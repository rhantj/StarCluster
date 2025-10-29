using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Space ship")]
    public Image spaceShipUpgradePanel;
    public List<Sprite> spaceShipSprites;
    public Transform upgradeRequirePos;
    public Button confirmButton;
    public GameObject itemSlot;
    UpgradeMinerals minerals;
    UpgradeData upgradeData;
    int upgrades = 0;

    void NextUpgrade()
    {
        foreach(Transform items in upgradeRequirePos)
        {
            if (items != null)
                Destroy(items.gameObject);
        }

        spaceShipUpgradePanel.sprite = spaceShipSprites[upgrades];
        var itemDatas = upgradeData.itemDatas;
        var itemCounts = upgradeData.itemCounts;

        for (int i = 0; i <= upgrades; ++i)
        {
            GameObject obj = Instantiate(itemSlot, upgradeRequirePos);
            obj.name = itemDatas[i].name;
            obj.GetComponent<Image>().sprite = itemDatas[i].Icon;
            obj.GetComponent<ItemSlot>().countText.text = itemCounts[i].ToString();

            obj.transform.SetParent(upgradeRequirePos, true);
        }
    }

    void OnConfirmBtnClicked()
    {
        if (upgrades == spaceShipSprites.Count - 1) return;
        RemoveItem();
        upgrades++;
        NextUpgrade();

    }

    void RemoveItem()
    {
        var itemDatas = upgradeData.itemDatas;
        var itemCounts = upgradeData.itemCounts;

        for (int i = 0; i <= upgrades; ++i)
        {
            //var slot = GetItemSlot(itemDatas[i]);
            var slot = GetItemSlot(itemDatas[i]);
            if (slot == null) continue;

            var countMinus = slot.count - itemCounts[i];
            if (countMinus < 0)
            {
                Debug.LogError("Mineral requirements are lacking");
                return;
            }
        }

        for (int i = 0; i <= upgrades; ++i)
        {
            var slot = GetItemSlot(itemDatas[i]);
            slot.count -= itemCounts[i];
        }
        
        UpdateUI();
    }

    private void Awake()
    {
        minerals = GetComponentInChildren<UpgradeMinerals>();
        upgradeData = minerals.GetUpgradeData();
    }

    private void OnEnable()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmBtnClicked);
    }

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
        NextUpgrade();
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
            emptySlot.count = data.Count;
            
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
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
        }
    }
}
