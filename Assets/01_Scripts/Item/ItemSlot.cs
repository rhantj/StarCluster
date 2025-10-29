using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public UpgradeControl inv;
    public Button button;
    public Image icon;
    public TextMeshProUGUI countText;
    Outline outline;

    public int idx;
    public int count;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = false;

        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickButton);
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.Icon;

        countText.text = count >= 1 ? count.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        countText.text = string.Empty;
    }

    public void OnClickButton()
    {
        inv.SelectItem(idx);
    }
}
