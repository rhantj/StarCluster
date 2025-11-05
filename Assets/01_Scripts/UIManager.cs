using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject titlePanel;

    [Header("Buttons")]
    [SerializeField] Button startBtn;
    [SerializeField] Button exitBtn;

    SceneManagement sma;
    public Action SetButtonAction;

    private void Awake()
    {
        sma = GetComponent<SceneManagement>();
        SetButtonAction += SetButtons;
    }

    private void OnDisable()
    {
        SetButtonAction -= SetButtons;
    }

    public void SetButtons()
    {
        startBtn.gameObject.SetActive(true);
        exitBtn.gameObject.SetActive(true);

        startBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

        startBtn.onClick.AddListener(OnStartButtonClicked);
        exitBtn.onClick.AddListener(OnExitButtonClicked);
    }

    void OnStartButtonClicked()
    {
        if(sma == null && sma.isLoading)
        {
            Debug.LogError("Scene Managemet is null or Loading");
            return;
        }

        titlePanel.SetActive(false);

        sma.LoadScene();
    }

    void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
