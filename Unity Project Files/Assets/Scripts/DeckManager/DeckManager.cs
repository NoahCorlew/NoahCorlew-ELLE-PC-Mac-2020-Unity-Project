using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System;
using Debug = UnityEngine.Debug;

// Manages the content of the deck, graveyard and exile
// The Collections in CollectionManager are the unchanging lists of cards present in the game at any time. These decks in this script are treated as normal decks, changing size and being drawn from.
public class DeckManager : MonoBehaviour
{
    // Graveyard and Exile are not used, lame.

    public static List<answer> answerDeck = new List<answer>();
    public static List<answer> answerGraveyard = new List<answer>();
    public static List<answer> answerExile = new List<answer>();

    public static List<question> questionDeck = new List<question>();
    public static List<question> questionGraveyard = new List<question>();
    public static List<question> questionExile = new List<question>();


    void Update()
    {
        if (questionDeck.Count == 0)
        {
            foreach (question q in questionGraveyard)
            {
                questionDeck.Add(q);
            }
            questionGraveyard.Clear();
            shuffle("question");
        }

        if (answerDeck.Count == 0)
        {
            foreach (answer a in answerGraveyard)
            {
                answerDeck.Add(a);
            }
            answerGraveyard.Clear();
            shuffle("answer");
        }
    }


    public static void createAnswerDeckFromCollection()
    {
        foreach (KeyValuePair<int, answer> answer in CollectionManager.answerCollection)
        {
            answerDeck.Add(answer.Value);
        }
        shuffle("answer");
    }

    public static void createQuestionDeckFromCollection()
    {
        foreach (KeyValuePair<int, question> question in CollectionManager.questionCollection)
        {
            questionDeck.Add(question.Value);
        }
        shuffle("question");
    }


    
    public static answer drawSmartAnswer()
    {
        // Gets list of all current active questions
        List<int> currentQuestionIDs = new List<int>();
        foreach (question q in QnAManager.currentQuestions)
        {
            foreach (int ID in q.getAnswerIDList())
            {
                currentQuestionIDs.Add(ID);
            }
        }

        // Gets list of current correct answers
        List<int> currentCorrectAnswerIDs = new List<int>();
        foreach (int i in currentQuestionIDs)
        {
            foreach (KeyValuePair<int, question> q in CollectionManager.questionCollection)
            { 
                if (currentQuestionIDs.Contains(q.Key))
                {
                    currentCorrectAnswerIDs.Add(q.Key);
                }
            }
        }

        // Loops through the list of current correct answers, finds one that is present in the answer deck, and draws the player that answer
        foreach (int i in currentCorrectAnswerIDs)
        {
            if (answerDeck.Contains(CollectionManager.answerCollection[i]))
            {
                answerDeck.Remove(CollectionManager.answerCollection[i]);
                return CollectionManager.answerCollection[i];
            }
        }
        return drawAnswer();
    }

    public static question drawSmartQuestion()
    {
        // Gets list of all current active questions
        List<int> currentQuestionIDs = new List<int>();
        foreach (question q in QnAManager.currentQuestions)
        {
            foreach (int ID in q.getAnswerIDList())
            {
                currentQuestionIDs.Add(ID);
            }
        }

        // Gets list of all current active answers
        List<int> currentAnswerIDs = new List<int>();
        foreach (answer a in QnAManager.currentAnswers)
        {
            currentAnswerIDs.Add(a.getAnswerID());
        }

        // Loops through the question deck, find a question that is answered by an active answer, then draws that question
        foreach (question q in questionDeck)
        {
            foreach(int i in currentAnswerIDs)
            {
                if (Array.Exists(q.getAnswerIDList(), element => element == i))
                {
                    questionDeck.Remove(q);
                    return q;
                }
            }
        }
        return drawQuestion();
    }

    public static int currentCorrectPairs()
    {
        // Gets list of all current active questions
        List<int> currentQuestionIDs = new List<int>();
        currentQuestionIDs.Clear();
        foreach (question q in QnAManager.currentQuestions)
        {
            foreach (int ID in q.getAnswerIDList())
            {
                if (ID != 0)
                    currentQuestionIDs.Add(ID);
            }
        }

        // Gets list of all current active answers
        List<int> currentAnswerIDs = new List<int>();
        currentAnswerIDs.Clear();
        foreach (answer a in QnAManager.currentAnswers)
        {
            if (a.getAnswerID() != 0)
                currentAnswerIDs.Add(a.getAnswerID());
        }

        // Gets the number of curent correct question answer pairs
        var commonElements = new List<int>();
        commonElements = currentAnswerIDs.Intersect(currentQuestionIDs).ToList();

        //Debug.Log("Current correct answer pairs: " + commonElements.Count());
        return commonElements.Count();
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

    public static List<int> arrayToListRemove0(int[] array)
    {
        List<int> list = new List<int>();
        foreach (int n in array)
        {
            if (n != 0)
                list.Add(n);
        }

        return list;
    }

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
                i = UnityEngine.Random.Range(0, size);
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
                i = UnityEngine.Random.Range(0, size);
                shuffle.Add(questionDeck.ElementAt(i));
                questionDeck.RemoveAt(i);
                size -= 1;
            }
            questionDeck = shuffle;
        }
    }

    /*
    public static List<int> shuffleCurrentAnswers(List<int> list)
    {
        List<int> shuffle = new List<int>();
        int size = list.Count;
        int i;
        while (size != 0)
        {
            i = UnityEngine.Random.Range(0, size);
            shuffle.Add(list.ElementAt(i));
            list.RemoveAt(i);
            size -= 1;
        }
        return shuffle;
    }
    */


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

    public static void addAnswerToGraveyard(int answerID)
    {
        answerGraveyard.Add(CollectionManager.answerCollection[answerID]);
    }

    public static void addQuestionToGraveyard(int questionID)
    {
        questionGraveyard.Add(CollectionManager.questionCollection[questionID]);
    }
}
