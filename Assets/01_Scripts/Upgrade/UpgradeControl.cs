using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
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
        selectedItem = null;

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
        StartCoroutine(Co_UpdateUI());
    }

    IEnumerator Co_UpdateUI()
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
                yield return null;
        }
    }

    void OnConfirmBtnClicked()
    {
        if (upgrades > spaceShipSprites.Count) return;
        RemoveItem();
    }

    void RemoveItem()
    {
        var itemDatas = upgradeData.itemDatas;
        var itemCounts = upgradeData.itemCounts;

        var i = 0;
        while (i < upgrades)
        {
            var slot = GetItemSlot(itemDatas[i]);
            if (slot == null)
            {
                Debug.LogError("slot is null");
                return;
            }

            if (slot.count < itemCounts[i])
            {
                Debug.LogError("Mineral requirements are lacking");
                return;
            }

            i++;
        }

        int j = 0;
        while (j < upgrades)
        {
            var slot = GetItemSlot(itemDatas[j]);
            slot.count -= itemCounts[j];

            Debug.Log(slot.count);

            if (slot.count <= 0)
            {
                slot.item = null;
                ClearSelectedItemWindow();
            }

            j++;
        }

        UpdateUI();
        UpgradeSpaceShip(upgrades - 1);
        NextUpgrade();
    }

    void UpgradeSpaceShip(int idx)
    {
        var ps = GameManager.Instance.GetSpaceShip();
        ps.GetComponent<PlayerSpaceShipContext>().Renderer.sprite = spaceShipSprites[idx];

        var speed = 8f + (8f / 4f) * (idx + 1);
        ps.GetComponent<PlayerSpaceShipControl>().SetMoveSpeed(speed);
    }

    void NextUpgrade()
    {
        foreach (Transform items in upgradeRequirePos)
        {
            if (items != null)
                Destroy(items.gameObject);
        }

        if (upgrades >= spaceShipSprites.Count - 1) return;

        spaceShipUpgradePanel.sprite = spaceShipSprites[upgrades];
        var itemDatas = upgradeData.itemDatas;
        var itemCounts = upgradeData.itemCounts;

        int i = 0;
        while (i < upgrades + 1)
        {
            GameObject obj = Instantiate(itemSlot, upgradeRequirePos);
            obj.name = itemDatas[i].name;
            var slot = obj.GetComponent<ItemSlot>();

            slot.icon.sprite = itemDatas[i].Icon;
            slot.countText.text = itemCounts[i].ToString();

            obj.transform.SetParent(upgradeRequirePos, true);
            i++;
        }

        upgrades++;
    }
}
