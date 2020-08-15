using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class QnAManager : MonoBehaviour
{
    public static List<attemptedAnswer> attemptedAnswers = new List<attemptedAnswer>(); // List of answers atempted
    public static List<question> currentQuestions = new List<question>();
    public static List<answer> currentAnswers = new List<answer>();

    private static bool correct;
    public static int points = 0;


    public static bool answerQuestion(int questionID, int answerID)
    {
        //Debug.Log("Attempting to answer questionID " + questionID + " with answerID " + answerID);

        correct = CollectionManager.questionCollection[questionID].answerQuestion(answerID);

        if (correct)
        {
            if (GameManager.timeBwtweenQuestions > 4)
                GameManager.timeBwtweenQuestions -= .04f;
            else if (GameManager.timeBwtweenQuestions > 3)
                GameManager.timeBwtweenQuestions -= .02f;
            else if (GameManager.timeBwtweenQuestions >= 2)
                GameManager.timeBwtweenQuestions -= 0;

            HealthBar.currentPercentHP += .05f;

            points++;

            if (CollectionManager.moduleCollection[GameManager.currentModule.getID()].getPB() < points)
            {
                Debug.Log("NEW PERSONAL RECORD");
                CollectionManager.moduleCollection[GameManager.currentModule.getID()].setPB(points);
            }
        }

        if (!correct)
        {
            HealthBar.currentPercentHP -= .25f;
        }

        return correct;
    }
}
