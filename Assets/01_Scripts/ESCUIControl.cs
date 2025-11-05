using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCUIControl : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject escPanel;

    [Header("Elements")]
    [SerializeField] Slider masterVolSlider;
    [SerializeField] Slider bgmVolSlider;
    [SerializeField] Slider sfxVolSlider;
    [SerializeField] Button escapeButton;
    [SerializeField] Button exitButton;

    [Header("Position")]
    static readonly Vector3 UI_HIDDEN_POS = new Vector3(2500f, 0, 0);

    [Header("Scene")]
    SceneManagement smt;

    [Header("Input")]
    DefaultInput inputAction;

    private void Awake()
    {
        smt = GetComponent<SceneManagement>();

        inputAction = new DefaultInput();
        inputAction.UI.ESC.started += ESC_started;

        inputAction.UI.Enable();
    }

    private void Start()
    {
        LoadSettings();
    }

    private void OnEnable()
    {
        masterVolSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolSlider.onValueChanged.AddListener(SetSFXVolume);

        escapeButton.onClick.AddListener(OnEscapeButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnDisable()
    {
        masterVolSlider.onValueChanged.RemoveAllListeners();
        bgmVolSlider.onValueChanged.RemoveAllListeners();
        sfxVolSlider.onValueChanged.RemoveAllListeners();

        escapeButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    void OnEscapeButtonClicked()
    {
        if (smt.isLoading) return;
        if (!GameManager.Instance.CurrentSceneName.Equals("Planet")) return;
        smt.LoadScene();

        escPanel.transform.localPosition = UI_HIDDEN_POS;
    }

    void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void ESC_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!obj.started) return;
        Toggle();
    }

    void Toggle()
    {
        if (SceneManager.GetActiveScene().name.Equals("Title Scene")) return;
        bool isOpen = escPanel.transform.localPosition == Vector3.zero;

        if (isOpen)
        {
            escPanel.transform.localPosition = UI_HIDDEN_POS;
        }
        else
        {
            escPanel.transform.localPosition = Vector3.zero;
        }
    }

    void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    void SetBGMVolume(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    void SetSFXVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }


    void LoadSettings()
    {
        masterVolSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume(masterVolSlider.value);
        SetBGMVolume(bgmVolSlider.value);
        SetSFXVolume(sfxVolSlider.value);
    }
}
