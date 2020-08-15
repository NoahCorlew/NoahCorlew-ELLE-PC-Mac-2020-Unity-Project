using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public static float pauseTimer = 0;

    void Update()
    {
        pauseTimer += Time.deltaTime;
    }

    public void unpause()
    {
        Debug.Log("YAY");
        MenuManager.pauseToggle = false;
    }

    public void returnMenu()
    {
        StartCoroutine(BackendHook.endSession(QnAManager.points));
        GameManager.resetStats();
        SceneManager.LoadScene("ModuleSelect");
    }

    public void exitGame()
    {
        StartCoroutine(BackendHook.endSession(QnAManager.points));
        Application.Quit();
    }
}
