using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startBtn;
    [SerializeField] Button exitBtn;

    SceneManagement sma;

    private void Awake()
    {
        sma = GetComponent<SceneManagement>();
    }

    private void Start()
    {
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
