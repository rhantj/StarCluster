using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearControl : MonoBehaviour
{
    public GameObject clearPanel;
    public Button confirmBtn;
    public Transform itemList;
    public int enemyCount;
    public GameObject itemSlot;
    SceneManagement smt;
    public List<ItemData> itemDatas = new();


    private void Awake()
    {
        gameObject.tag = "Canvas";
        smt = GetComponent<SceneManagement>();
    }

    private void OnEnable()
    {
        confirmBtn.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        confirmBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        itemDatas = GameManager.Instance.GetPlanetItemData();
    }

    public void Toggle()
    {
        if (enemyCount > 0) return;

        if (!clearPanel.activeSelf)
        {
            UpdateUI();
            clearPanel.SetActive(true);
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < itemDatas.Count; ++i)
        {
            GameObject obj = Instantiate(itemSlot, itemList);
            obj.name = itemDatas[i].name;
            obj.GetComponent<Image>().sprite = itemDatas[i].Icon;
            obj.GetComponent<ItemSlot>().countText.text = itemDatas[i].Count.ToString();

            obj.transform.SetParent(itemList, true);
        }
    }

    public void PlusEnemyCount()
    {
        enemyCount++;
    }

    public void MinusEnemyCount()
    {
        enemyCount--;
    }

    void OnButtonClicked()
    {
        if (confirmBtn == null) return;
        if (smt.isLoading) return;

        smt.LoadScene();
    }
}
