using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public TMP_Dropdown ResolutionDropdown;
    public TMP_Dropdown WindowModeDropDown;
    public bool SettingsOpen = false;

    public string gameSceneName = "GameScene";

    private List<Resolution> uniqueResolutions = new List<Resolution>();

    void Start()
    {
        //Filter res to remove duplicates idk i got this shit from stack i hope it works dont ask me any questions 
        uniqueResolutions = Screen.resolutions
        .GroupBy(r => new { r.width, r.height })
        .Select(g => g.First())
        .ToList();

        //GetResolutions
        ResolutionDropdown.ClearOptions();
        var Options = uniqueResolutions.Select(r => $"{r.width} x {r.height}").ToList();
        ResolutionDropdown.AddOptions(Options);

        //SetDefault
        Resolution currentRes = Screen.currentResolution;
        int defaultIndex = uniqueResolutions.FindIndex(r => r.width == currentRes.width && r.height == currentRes.height);
        ResolutionDropdown.value = defaultIndex >= 0 ? defaultIndex : 0;
        ResolutionDropdown.RefreshShownValue();

        //FillWindowDropDown
        WindowModeDropDown.ClearOptions();
        WindowModeDropDown.AddOptions(new List<string> { "Fullscreen", "Windowed Fullscreen", "Windowed" });

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                WindowModeDropDown.value = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                WindowModeDropDown.value = 1;
                break;
            case FullScreenMode.Windowed:
                WindowModeDropDown.value = 2;
                break;
        }
        WindowModeDropDown.RefreshShownValue();

        //Listeners
        ResolutionDropdown.onValueChanged.AddListener(SetResolution);
        WindowModeDropDown.onValueChanged.AddListener(SetWindowMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (SettingsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();

        }
    }

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
        Debug.Log("Play");
    }

    public void Exit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
        SettingsOpen = true;
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
        SettingsOpen = false;

    }

    private void SetResolution(int index)
    {
        Resolution res = uniqueResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    private void SetWindowMode(int modeIndex)
    {
        switch (modeIndex)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

        }
    }
}
