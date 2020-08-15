using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;



public class QuestionTimer : MonoBehaviour
{
    float amnt;
    public Image circleTimer;

    void Update()
    {

        if (GameManager.gameLost == false)
        {
            amnt = GameManager.timeBwtweenQuestions - GameManager.timer;
            circleTimer.fillAmount = (amnt / GameManager.timeBwtweenQuestions);
        }
        else if (GameManager.gameLost == true)
        {
            circleTimer.fillAmount = 0;
        }
    }
}
