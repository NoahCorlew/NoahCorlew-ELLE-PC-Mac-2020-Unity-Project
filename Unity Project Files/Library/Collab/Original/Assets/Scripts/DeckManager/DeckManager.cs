using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

// Manages the content of the deck, graveyard and exile
public class DeckManager : MonoBehaviour
{
    // Graveyard and Exile are not used, lame.

    public static List<answer> answerDeck = new List<answer>();
    public static List<answer> answerGraveyard = new List<answer>();
    public static List<answer> answerExile = new List<answer>();

    public static List<question> questionDeck = new List<question>();
    public static List<question> questionGraveyard = new List<question>();
    public static List<question> questionExile = new List<question>();

    public static void createAnswerDeckFromCollection()
    {
        foreach (KeyValuePair<int, answer> answer in CollectionManager.answerCollection)
        {
            answerDeck.Add(answer.Value);
        }
    }

    public static void createQuestionDeckFromCollection()
    {
        foreach (KeyValuePair<int, question> question in CollectionManager.questionCollection)
        {
            questionDeck.Add(question.Value);
        }
    }

    public static answer drawAnswer()
    {
        answer temp = answerDeck[0];
        answerDeck.RemoveAt(0);

        return temp;
    }

    public static question drawQuestion()
    {
        question temp = questionDeck[0];
        questionDeck.RemoveAt(0);

        return temp;
    }

    /*
    public static answer drawSmartAnswer()
    {


    }
    */

    // type: answer or question
    public static void shuffle(string type)
    {
        if (type == "answer")
        {
            //Debug.Log("Shuffling answer deck");
            List<answer> shuffle = new List<answer>();
            int size = answerDeck.Count;
            int i;
            while (size != 0)
            {
                i = Random.Range(0, size);
                shuffle.Add(answerDeck.ElementAt(i));
                answerDeck.RemoveAt(i);
                size -= 1;
            }
            answerDeck = shuffle;
        }
        else if (type == "question")
        {
            //Debug.Log("Shuffling question deck");
            List<question> shuffle = new List<question>();
            int size = questionDeck.Count;
            int i;
            while (size != 0)
            {
                i = Random.Range(0, size);
                shuffle.Add(questionDeck.ElementAt(i));
                questionDeck.RemoveAt(i);
                size -= 1;
            }
            questionDeck = shuffle;
        }
        //Debug.Log("Done");
    }


    // type: answer or question
    // stack: deck, graveyard or exile
    public static int getDeckSize(string type, string stack)
    {
        if (type == "question")
        {
            if (stack == "deck")
            {
                return questionDeck.Count;
            }
            if (stack == "graveyard")
            {
                return questionGraveyard.Count;
            }
            if (stack == "exile")
            {
                return questionExile.Count;
            }

        }
        else if (type == "answer")
        {
            if (stack == "deck")
            {
                return answerDeck.Count;
            }
            if (stack == "graveyard")
            {
                return answerGraveyard.Count;
            }
            if (stack == "exile")
            {
                return answerExile.Count;
            }
        }
        Debug.Log("Done");

        return -1;
    }


    // Outputs the content of a deck
    // type: answer or question
    // stack: deck, graveyard or exile
    public void showStack(string type, string stack)
    {
        if (type == "question")
        {
            if (stack == "deck")
            {
                Debug.Log("Printing Question Deck");
                foreach (question question in questionDeck)
                {
                    Debug.Log("QuestionID: " + question.getQuestionID());
                    Debug.Log("Qu Answers: " + question.getAnswerIDList().ToString());
                }
            }
            if (stack == "graveyard")
            {
                Debug.Log("Printing Question Graveyard");
                foreach (question question in questionGraveyard)
                {
                    Debug.Log("QuestionID: " + question.getQuestionID());
                    Debug.Log("Qu Answers: " + question.getAnswerIDList().ToString());
                }
            }
            if (stack == "exile")
            {
                Debug.Log("Printing Question Exile");
                foreach (question question in questionExile)
                {
                    Debug.Log("QuestionID: " + question.getQuestionID());
                    Debug.Log("Qu Answers: " + question.getAnswerIDList().ToString());
                }
            }

        }
        else if (type == "answer")
        {
            if (stack == "deck")
            {
                Debug.Log("Printing Answer Deck");
                foreach (answer answer in answerDeck)
                {
                    Debug.Log("AnswerID: " + answer.getAnswerID());
                    Debug.Log("AnsPhras: " + answer.getAnswerPhrase());
                }
            }
            if (stack == "graveyard")
            {
                Debug.Log("Printing Answer Graveyard");
                foreach (answer answer in answerGraveyard)
                {
                    Debug.Log("AnswerID: " + answer.getAnswerID());
                    Debug.Log("AnsPhras: " + answer.getAnswerPhrase());
                }
            }
            if (stack == "exile")
            {
                Debug.Log("Printing Answer Exile");
                foreach (answer answer in answerExile)
                {
                    Debug.Log("AnswerID: " + answer.getAnswerID());
                    Debug.Log("AnsPhras: " + answer.getAnswerPhrase());
                }
            }
        }
        Debug.Log("Done");
    }

    public void addAnswerToGraveyard(int answerID)
    {
        answerGraveyard.Add(CollectionManager.answerCollection[answerID]);
    }

    public void addQuestionToGraveyard(int questionID)
    {
        questionGraveyard.Add(CollectionManager.questionCollection[questionID]);
    }
}
