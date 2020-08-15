using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;


// Rename to hand manager
public class HandPhysicsManager : MonoBehaviour
{
    // Drawing Cards
    public static bool drawBool;

    // Moving cards
    public static bool cardIsGrabbed = false;
    public static bool cardHasPriority = false;

    // Spawning Cards
    public GameObject testCardPF;
    private GameObject spawnedCard;
    private int smallestOpenNode;
    private List<int> takenHandNodes = new List<int>();

    // Spawning and moving Nodes
    public GameObject handNodePF;
    private GameObject spawnedNode;
    public static int spaceBetweenNodes;
    public static float nodeSpacing;
    public static float firstNodeX;
    private static int nodeCounter;
    public static int currentPresentNodeCount = 0;
    public static bool moveNodesFlag;
    public static int answerCount;
    public static int handNodeCount;
    private int chance;

    //Gameprops
    public int maxCards = 7;
    public int maxNodes = 11;

    [HideInInspector]
    public static GameObject[] handNodeGOs;
    public static GameObject[] cardGOs;
    public static List<GameObject> cardGOlist = new List<GameObject>(); // List used for managing priority view of cards in hand when they overlap. Punishment for using arrays instead of lists.
    
    void Start()
    {
        answerCount = CollectionManager.answerCollection.Count;

        if (answerCount > 7)
            handNodeCount = 4;
        else if (answerCount > 10)
            handNodeCount = 5;
        else
            handNodeCount = 3;

        // Creates the nodes to be moved later 
        createNodes(handNodeCount);
        findCards();
        //CollectionManager.printCollection();
    }

    void Update()
    {
        if (drawBool)
        {
            drawBool = false;

            if (cardGOs.Length == 0)
                chance = UnityEngine.Random.Range(0, DeckManager.currentCorrectPairs());

            if (chance == 0)
                Draw(DeckManager.drawSmartAnswer());
            else
                Draw(DeckManager.drawAnswer());
        }

        if (currentPresentNodeCount < handNodeCount)
            Draw(DeckManager.drawAnswer());
    }

    void Draw(answer drawnAnswer)
    {

        // I hate myself, and if you're reading this, you hate me too.

        if (currentPresentNodeCount < handNodeCount)
        {
            moveNodes(currentPresentNodeCount + 1);
            // Finds the smallest open node and sets the newly spawned card to be at that node
            takenHandNodes.Clear();

            foreach (GameObject card in cardGOs)
            {
                takenHandNodes.Add(card.GetComponent<CardPhysics>().currentHandNode);
            }

            for (int i = maxNodes; i >= 0; i--)
            {
                if (!takenHandNodes.Contains(i))
                    smallestOpenNode = i;
            }
        }
        else
            smallestOpenNode = currentPresentNodeCount - 1;


        spawnedCard = Instantiate(testCardPF, new Vector2(-30, 0), Quaternion.identity);

        spawnedCard.GetComponent<CardPhysics>().currentHandNode = smallestOpenNode;
        spawnedCard.transform.parent = GameObject.Find("CardObjects").transform;


        if (UnityEngine.Random.Range(0, 2) == 1)
            spawnedCard.GetComponent<AnswerObject>().answer = drawnAnswer;
        else
            spawnedCard.GetComponent<AnswerObject>().answer = drawnAnswer;


        spawnedCard.GetComponent<AnswerObject>().answerID = spawnedCard.GetComponent<AnswerObject>().answer.getAnswerID();
        spawnedCard.GetComponent<AnswerObject>().answerText = spawnedCard.GetComponent<AnswerObject>().answer.getAnswerPhrase();

        // Adds the drawn answer to the list of current answers
        QnAManager.currentAnswers.Add(spawnedCard.GetComponent<AnswerObject>().answer);

        findCards();
    }

    void createNodes(int nodeCount)
    {
        // Spawns the handNodes for the first time, only needs to be run once
        for (float i = 0; i != nodeCount; i++)
        {
            spawnedNode = Instantiate(handNodePF, new Vector2(-100, -100), Quaternion.identity);
            spawnedNode.transform.parent = GameObject.Find("HandNodes").transform;
        }

        handNodeGOs = GameObject.FindGameObjectsWithTag("HandNode");
    }

    // Moves and disables nodes depending on hand size
    public static void moveNodes(int nodeCount)
    {
        currentPresentNodeCount = nodeCount;
        switch (nodeCount)
        {
            case 1:
                firstNodeX = 0f;
                nodeSpacing = 5f;

                break;
            case 2:
                firstNodeX = .5f;
                nodeSpacing = 5f;

                break;
            case 3:
                firstNodeX = 1;
                nodeSpacing = 5f;

                break;
            case 4:
                firstNodeX = 1.5f;
                nodeSpacing = 5f;

                break;
            case 5:
                firstNodeX = 2f;
                nodeSpacing = 5f;

                break;
            case 6:
                firstNodeX = 2.5f;
                nodeSpacing = 5f;

                break;
            case 7:
                firstNodeX = 3f;
                nodeSpacing = 4f;

                break;
            case 8:
                firstNodeX = 3.4f;
                nodeSpacing = 3f;

                break;
            case 9:
                firstNodeX = 4.5f;
                nodeSpacing = 3f;

                break;
            case 10:
                firstNodeX = 4.5f;
                nodeSpacing = 3f;

                break;
        }

        nodeCounter = 0;
        foreach (GameObject handNode in handNodeGOs)
        {
            if (nodeCounter < nodeCount)
            {
                handNode.SetActive(true);
                handNode.transform.position = new Vector2(Mathf.Floor(-firstNodeX * nodeSpacing) + nodeCounter * (nodeSpacing), 0);
                handNode.GetComponent<HandNodeID>().nodeID = nodeCounter;
            }
            else if (nodeCounter >= nodeCount)
            {
                handNode.SetActive(false);
                handNode.transform.position = new Vector2(-100, -100);
            }
            nodeCounter++;
        }
    }

    // Updates the list of card gameobjects
    public static void findCards()
    {
        cardGOs = GameObject.FindGameObjectsWithTag("Card");
        cardGOlist.AddRange(cardGOs);
    }


    // Moves all cards with currentHandNode between a and b to the right
    public static void moveAllCardsRightBetween(int a, int b)
    {
        foreach (GameObject card in cardGOs)
        {
            if (card.GetComponent<CardPhysics>().currentHandNode >= a && card.GetComponent<CardPhysics>().currentHandNode <= b)
                card.GetComponent<CardPhysics>().moveRightFlag = true;
        }
    }
    
    // Moves all cards with currentHandNode between a and b to the left
    public static void moveAllCardsLeftBetween(int a, int b)
    {
        foreach (GameObject card in cardGOs)
        {
            if (card.GetComponent<CardPhysics>().currentHandNode >= a && card.GetComponent<CardPhysics>().currentHandNode <= b)
                card.GetComponent<CardPhysics>().moveLeftFlag = true;
        }
    }
}
