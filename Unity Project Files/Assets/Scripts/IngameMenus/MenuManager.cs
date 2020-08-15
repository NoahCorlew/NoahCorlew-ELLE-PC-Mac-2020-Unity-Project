using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static bool pauseToggle = false;
    public GameObject pauseMenu;
    public GameObject loseMenu;

    public static bool turnOffGameMenusFlag = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.gameLost)
        {
            pauseToggle = !pauseToggle;
        }

        if (turnOffGameMenusFlag)
        {
            loseMenu.SetActive(false);
            pauseMenu.SetActive(false);

            turnOffGameMenusFlag = false;
        }

        if (!pauseToggle)
        {
            pauseMenu.SetActive(false);

            foreach (GameObject cardGO in HandPhysicsManager.cardGOs)
            {
                try
                {
                    cardGO.GetComponent<BoxCollider2D>().enabled = true;
                }
                catch
                {
                
                }
            }

        }
        else
        {
            pauseMenu.SetActive(true);
            foreach (GameObject cardGO in HandPhysicsManager.cardGOs)
            {
                try
                {
                    cardGO.GetComponent<BoxCollider2D>().enabled = false;
                }
                catch
                { }
            }
        }

        if (GameManager.gameLost)
        {
            loseMenu.SetActive(true);
            foreach (GameObject cardGO in HandPhysicsManager.cardGOs)
            {
                try
                {
                    cardGO.GetComponent<BoxCollider2D>().enabled = false;
                }
                catch
                { }
            }
        }
    }
}
