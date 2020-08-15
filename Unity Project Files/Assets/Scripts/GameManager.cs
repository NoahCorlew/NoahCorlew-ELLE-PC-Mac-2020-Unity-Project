using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using System;
using Debug = UnityEngine.Debug;



public class GameManager : MonoBehaviour
{
    public static bool resetStatsSorryIHateMeToo = false;
    public static int startingCards;
    public static float timeBwtweenQuestions;
    public static float timer;
    public static int questionCount;
    public static int answerCount;
    public static ModuleInfo currentModule;
    public static bool endlessMode = false;
    public static bool pictureMode = false;
    private bool destroyedQT = false;
    private bool destroyedLM = false;

    public static bool gameLost = false;

    IEnumerator Start()
    {
        // saves the images to the questoins and answers
        // Move to moduleselect
        // have stack that incriments whenever a new corutine is called and decrements when one is complete, wait til it is at 0


        if (pictureMode)
            foreach (KeyValuePair<int, answer> entry in CollectionManager.answerCollection)
            {
                StartCoroutine(BackendHook.downloadImg(entry.Value.getImageLocation(), false, entry.Key));
            }

        CollectionManager.createCardCollection();

        // Creates the answer and question decks from the collection
        DeckManager.createAnswerDeckFromCollection();
        DeckManager.createQuestionDeckFromCollection();
        DeckManager.shuffle("answer");

        // Spawn the question nodes. Having trouble moving that from its own start function to here

        // Spawn the hand nodes. Same as above.
        //HandPhysicsManager.init();
        //HandPhysicsManager.createNodes(HandPhysicsManager.maxNodes);

        questionCount = CollectionManager.questionCollection.Count;
        answerCount = CollectionManager.answerCollection.Count;

 

        return null;
    }


    void Update()
    {
        if (!destroyedQT && endlessMode && GameObject.Find("QuestionTimer") != null)
        {
            Destroy(GameObject.Find("QuestionTimer"));
            destroyedQT = true;
        }
        
        if (!destroyedLM && endlessMode && GameObject.Find("LifeManager") != null)
        {
            Destroy(GameObject.Find("LifeManager"));
            destroyedLM = true;
        }


        if (!MenuManager.pauseToggle && !gameLost && !(QuestionDropsManager.questionDropGOs.Length == 4 && endlessMode))
        {
            if (!endlessMode)
                timer += Time.deltaTime;

            if (endlessMode)
                timer += Time.deltaTime * 1.5f;

            // Adds playtime to the overall playtime
            currentModule.addTime(Time.deltaTime);
        }

        // Spawns questions
        if (timer > timeBwtweenQuestions && !gameLost)
        {
            timer = 0;
            // Handles loss from question overload
            if (QuestionDropsManager.questionDropGOs.Length == 4 && !endlessMode)
            {
                Debug.Log("Game lost from question overload");
                gameLost = true;
            }

            if (!gameLost)
            {
                QuestionDropsManager.createSmartQuestion = true;
                QuestionDropsManager.createQuestion = true;
            }
        }

        // If there are no questions, speed up the timer until one spawns
        if (!MenuManager.pauseToggle && QuestionDropsManager.questionDropGOs.Length == 0)
            timer += Time.deltaTime * 3;

        // Handles loss from lack of health
        if (HealthBar.currentPercentHP <= 0 && !endlessMode)
        {
            gameLost = true;
        }
    }
    public static void resetStats()
    {
        clear(QuestionDropsManager.questionDropGOs);
        clear(HandPhysicsManager.handNodeGOs);
        clear(HandPhysicsManager.cardGOs);

        HandPhysicsManager.currentPresentNodeCount = 0;
        HandPhysicsManager.answerCount = 0;
        HandPhysicsManager.handNodeCount = 0;
        HandPhysicsManager.cardGOlist.Clear();

        DeckManager.answerDeck.Clear();
        DeckManager.questionDeck.Clear();
        DeckManager.answerGraveyard.Clear();
        DeckManager.questionGraveyard.Clear();

        BackendHook.collectionMade = false;

        CollectionManager.moduleCollection.Clear();
        CollectionManager.questionCollection.Clear();
        CollectionManager.answerCollection.Clear();
        
        ModuleSelect.loadOnce = true;
        MenuManager.pauseToggle = false;

        pictureMode = false;
        endlessMode = false;
        gameLost = false;
        HealthBar.currentPercentHP = 1;
        timeBwtweenQuestions = 5;
        timer = 0;
        QuestionDropsManager.createQuestion = false;
        HandPhysicsManager.drawBool = false;

        PauseScript.pauseTimer = 0;
        Time.timeScale = 1;
    }


    public static void clear(GameObject[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = null;
        }
    }

}
