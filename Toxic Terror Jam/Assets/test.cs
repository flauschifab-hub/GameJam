using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
          SceneManager.LoadScene(gameSceneName);
        }
    }
}
