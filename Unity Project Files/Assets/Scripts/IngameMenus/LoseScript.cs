using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class LoseScript : MonoBehaviour
{
    public GameObject loseWindow;

    void OnEnable()
    {
        StartCoroutine(BackendHook.endSession(QnAManager.points));
    }

    public void returnToMenu()
    {
        loseWindow.SetActive(false);
        GameManager.resetStats();
        SceneManager.LoadScene("ModuleSelect");
    }
}
