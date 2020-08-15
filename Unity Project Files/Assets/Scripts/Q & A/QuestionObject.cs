using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// Connects the answer in the answerCollection dictionary in BackendHook to the physical drop point
// questionID = key in dictionary for the answer
public class QuestionObject : MonoBehaviour
{
    public Text questionText;

    public int questionPos;
    public int questionID;
    public question question;
    public float redTimer = 0;

    public string questionPhrase;

    public int[] correctAnswers;
    public float[] cardDistances = new float[20];

    void Start()
    {
        // Sets the public list of correct answers on this node using the questionCollection dictionary 
        //Debug.Log("Attempting to make question with ID " + questionID);
        correctAnswers = CollectionManager.questionCollection[questionID].getAnswerIDList();
        questionPhrase = question.getQuestionPhrase();
        questionText.text = questionPhrase;
    }

    void FixedUpdate()
    {
        if (redTimer > 0)
        {
            questionText.color = Color.red;
            redTimer -= Time.deltaTime;
        }
        else if (distanceToClosestAnswerCard() > 4)
        {
            questionText.color = Color.grey;
        }

    }


    public float distanceToClosestAnswerCard()
    {
        Vector2 nodeCoords;
        float smallestDistance = 50000;
        float distance;
        int closestCard = -1; // Card ID must never be -1
        foreach (GameObject aGOs in HandPhysicsManager.cardGOs)
        {
            nodeCoords = aGOs.transform.position;
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, this.transform.position.x, nodeCoords.y);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
            }
        }

        return smallestDistance;
    }

    public float distanceBetween(float x1, float y1, float x2, float y2)
    {
        return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
}
