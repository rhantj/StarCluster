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
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;

    [Header("Space ship")]
    public Image spaceShipUpgradePanel;
    public List<Sprite> spaceShipSprites;
    public Transform upgradeRequirePos;
    public Button confirmButton;
    public GameObject itemSlot;
    public UpgradeData upgradeData;
    public int upgrades = 0;

    private void Awake()
    {
        if (UpgradeSaveLoad.TryLoadJson(out var data))
        {
            upgrades = data.upgrades;
        }
    }

    private void Start()
    {
        StartCoroutine(UpgradeShipBeforeStart());

        invWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].idx = i;
            slots[i].inv = this;

            ClearSelectedItemWindow();
        }

        LoadInventorySlots();
        UpdateUI();
        NextUpgrade();

        confirmButton.onClick.AddListener(OnConfirmBtnClicked);
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

        selectedItemName.text = selectedItem.item.Name;
        selectedItemDescription.text = selectedItem.item.Description;
    }

    ItemData MakeReferenceEqual(ItemData data)
    {
        if (data == null) return null;
        foreach(var d in upgradeData.itemDatas)
        {
            if (d != null && d.name == data.name) return d;
        }
        return data;
    }

    public void AddItem(ItemData data)
    {
        data = MakeReferenceEqual(data);
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
        if (data == null) return null;

        for (int i = 0; i < slots.Length; ++i)
        {
            var item = slots[i].item;
            if (item == null) continue;

            if (ReferenceEquals(item, data)) return slots[i];
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
            else
            {
                slots[i].Clear();
            }
        }

        SaveInventorySlots();
    }

    void OnConfirmBtnClicked()
    {
        RemoveItem();
    }

    void RemoveItem()
    {
        if (upgrades >= spaceShipSprites.Count) return;

        var itemDatas = upgradeData.itemDatas;
        var itemCounts = upgradeData.itemCounts;

        int upgradeCount = Mathf.Min(upgrades + 1, itemDatas.Count);

        for (int i = 0; i < upgradeCount; ++i)
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
        }

        for (int i = 0; i < upgradeCount; ++i)
        {
            var slot = GetItemSlot(itemDatas[i]);
            slot.count -= itemCounts[i];

            if (slot.count <= 0)
            {
                slot.item = null;
                ClearSelectedItemWindow();
            }
        }

        UpdateUI();
        UpgradeSpaceShip(upgrades);
        upgrades++;
        NextUpgrade();

        SaveInventorySlots();
    }

    void UpgradeSpaceShip(int idx)
    {
        var ps = GameManager.Instance.GetSpaceShip();
        ps.GetComponent<PlayerSpaceShipContext>().Renderer.sprite = spaceShipSprites[idx];

        var psSpeed = ps.GetComponent<PlayerSpaceShipControl>().moveSpeed;

        var speed = psSpeed + (psSpeed * 0.25f) * (idx + 1);
        ps.GetComponent<PlayerSpaceShipControl>().SetMoveSpeed(speed);
    }

    IEnumerator UpgradeShipBeforeStart()
    {
        yield return new WaitUntil(() => ObjectPoolManager.Instance.IsReady);

        if (upgrades > 0)
        {
            ObjectPoolManager.Instance.GetObjectFromPool("Player_SpaceShip", out var ps);
            ps.GetComponent<PlayerSpaceShipContext>().Renderer.sprite = spaceShipSprites[upgrades - 1];

            var psSpeed = ps.GetComponent<PlayerSpaceShipControl>().moveSpeed;

            var speed = psSpeed + (psSpeed * 0.25f) * upgrades;
            ps.GetComponent<PlayerSpaceShipControl>().SetMoveSpeed(speed);
        }
        else yield break;
    }

    void NextUpgrade()
    {
        for (int i = upgradeRequirePos.childCount - 1; i >= 0; --i)
        {
            var c = upgradeRequirePos.GetChild(i);
            if (c != null) Destroy(c.gameObject);
        }

        if (upgrades >= spaceShipSprites.Count) return;

        spaceShipUpgradePanel.sprite = spaceShipSprites[upgrades];
        var itemDatas = upgradeData.itemDatas;
        var itemCounts = upgradeData.itemCounts;

        int upgradeCount = Mathf.Min(upgrades + 1, itemDatas.Count);

        for (int i = 0; i < upgradeCount; ++i) 
        {
            GameObject obj = Instantiate(itemSlot, upgradeRequirePos);
            obj.name = itemDatas[i].name;
            var slot = obj.GetComponent<ItemSlot>();

            slot.icon.sprite = itemDatas[i].Icon;
            slot.countText.text = itemCounts[i].ToString();

            obj.transform.SetParent(upgradeRequirePos, false);
        }
    }

    void SaveInventorySlots()
    {
        var json = new UpgradeJson
        {
            upgrades = upgrades,
            jsonSlots = new List<ItemSlotJson>(slots.Length),
        };

        for (int i = 0; i < slots.Length; ++i)
        {
            var slot = slots[i];
            var jsonSlot = new ItemSlotJson
            {

                itemName = slot.item != null ? slot.item.name : string.Empty,
                count = slot.item != null ? slot.count : 0
            };

            json.jsonSlots.Add(jsonSlot);
        }

        UpgradeSaveLoad.Save(json);
    }

    void LoadInventorySlots()
    {
        if (!UpgradeSaveLoad.TryLoadJson(out var data))
        {
            upgrades = 0;
            foreach(var slot in slots)
            {
                slot.item = null;
                slot.count = 0;     
            }
            return;
        }

        upgrades = data.upgrades;

        int cnt = data.jsonSlots.Count;
        for (int i = 0; i < data.jsonSlots.Count; ++i)
        {
            var jsonslot = data.jsonSlots[i];
            if (string.IsNullOrEmpty(jsonslot.itemName) || jsonslot.count <= 0)
            {
                slots[i].item = null;
                slots[i].count = 0;
                continue;
            }

            ItemData idata = null;
            foreach(var d in upgradeData.itemDatas)
            {
                if (d != null && d.name.Equals(jsonslot.itemName))
                {
                    idata = d;
                    break;
                }
            }

            slots[i].item = idata;
            slots[i].count = jsonslot.count;
        }

        for (int i = cnt; i < data.jsonSlots.Count; ++i)
        {
            slots[i].item = null;
            slots[i].count = 0;
        }
    }
}
