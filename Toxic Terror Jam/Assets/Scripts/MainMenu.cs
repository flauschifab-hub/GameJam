using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;

    public bool SettingsOpen = false;

    public string gameSceneName = "GameScene";

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
}
