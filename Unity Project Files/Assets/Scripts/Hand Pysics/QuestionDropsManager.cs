using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// Manages question drop areas and any other drop areas
// Manages location of question drop area locations

public class QuestionDropsManager : MonoBehaviour
{
    public static bool createSmartQuestion = true;
    public static bool createQuestion = true;
    private int chance;

    public static bool deletedFlag;
    public GameObject questionDropNode;
    private GameObject newQuestionDropArea;

    public static GameObject[] questionDropGOs;

    void Start()
    {
        questionDropGOs = GameObject.FindGameObjectsWithTag("QuestionDropNode");
    }

    void Update()
    {
        if (createQuestion && questionDropGOs.Length != 4)
        {
            createQuestion = false;

            chance = Random.Range(0, DeckManager.currentCorrectPairs());
           
            if (chance == 0 && questionDropGOs.Length != 0)
                createQuestionDropArea(DeckManager.drawSmartQuestion());
            else
                createQuestionDropArea(DeckManager.drawQuestion());
        }
    }


    // Only called once, when moving an answer node call moveQuestionDropArea(...)
    public void createQuestionDropArea(question question)
    {
        // Creates the QDA
        newQuestionDropArea = Instantiate(questionDropNode, new Vector2(0, 14 - (questionDropGOs.Length*3)), Quaternion.identity); // -14, 16

        // Sets the value of the question in QuestionObject.cs script, which each physical QDA has, to be the question created in QuestionDropManager.cs
        newQuestionDropArea.GetComponent<QuestionObject>().question = question;
        newQuestionDropArea.GetComponent<QuestionObject>().questionPos = questionDropGOs.Length;

        // Adds the new question to the list of current questions
        QnAManager.currentQuestions.Add(question);

        findQuestionDropAreas();
    }

    public static void deleteQuestionDropArea(int questionID)
    {
        DeckManager.questionDeck.Add(CollectionManager.questionCollection[questionID]);

        deletedFlag = false;
        float yPosOfQuestionToDelete = 0;
        foreach (GameObject cardDropArea in questionDropGOs)
        {
            if (cardDropArea.GetComponent<QuestionObject>().questionID == questionID && deletedFlag == false)
            {
                yPosOfQuestionToDelete = cardDropArea.transform.position.y;
                cardDropArea.tag = "Untagged";
                Destroy(cardDropArea);
                deletedFlag = true;
            }
        }

        findQuestionDropAreas();

        foreach (GameObject cardDropArea in questionDropGOs)
        {
            if (cardDropArea.transform.position.y < yPosOfQuestionToDelete)
            {
                cardDropArea.transform.position = new Vector2(cardDropArea.transform.position.x, cardDropArea.transform.position.y + 3);
            }
        }

        findQuestionDropAreas();
    }

    // Refreshes the list of question drop gameobjects
    public static void findQuestionDropAreas()
    {
        questionDropGOs = GameObject.FindGameObjectsWithTag("QuestionDropNode");

        // Sets the ID of each questionDropArea to be the ID of its question
        foreach(GameObject cardDropArea in questionDropGOs)
        {
            cardDropArea.GetComponent<QuestionObject>().questionID = cardDropArea.GetComponent<QuestionObject>().question.getQuestionID();
        }
    }
}
