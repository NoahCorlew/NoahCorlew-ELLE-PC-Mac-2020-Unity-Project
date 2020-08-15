using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.Events; 
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
using System.Threading;

public class CardPhysics : MonoBehaviour
{
    // Important todo: Change system to use card class that has currentHandNode and other card attributes like word

    public int currentHandNode;             // Current node the card is attatched to 
    private Vector2 location;               // Current position of card
    private Vector2 destination;            // Current location of the node the card is attatched to
    public bool moveLeftFlag;               // Flags set in HandPhysicsManager, used to quickly tell multiple cards to move
    public bool moveRightFlag;
    private bool alreadyRan1 = false;        // Flag used to prevent a picked up card from moving other cards more than once
    private bool alreadyRan2 = false;        // Similar to above but for moving cards left when card on mouse is near question
    public int openNode;                    // Variable used to hold location of a card that just had its currentHandNode changed, causing an empty space to appear. Used to determine if cards should be moved left or right.
    private int originalNode;               // Stores original location of card before it is moved at all. Used when the player tries to answer a question but is wrong.
    private bool movingCards;               // Flag used to prevent setPosition() from running when the user is moving cards.
    private bool priorityView = false;      // Flag used to designate a card being hovered over. All other cards have this flag turned off when a card is hovered over to achieve this.
    private string nodeNav;                 // Flag used to tell setPosition() what to do. hand = nav to hand, question = nav to question
    private bool thisCardCollision = true;
    private bool pictureCard = false;
    private bool pictureSet = false;

    // Variables for click and drag system
    private Plane dragPlane;
    private Vector3 offset;
    private Camera myMainCamera;
    private Vector3 mouse;

    // Card Art
    public Text cardText;
    public RawImage cardArt;
    public Texture2D test;


    void Start()
    {
        if (UnityEngine.Random.value < .5 && GameManager.pictureMode)
        {
            pictureCard = true;
        }

        cardArt.CrossFadeAlpha(0, 0f, false);


        nodeNav = "hand";
        myMainCamera = Camera.main;

        // Sets the card text, if the images loads because pictureCard is true, this text is set to blank
        cardText.text = this.GetComponent<AnswerObject>().answerText.ToString();
        
    }

    void FixedUpdate()
    {

        if (pictureCard && !pictureSet)
        {
            if (File.Exists(Application.persistentDataPath + "/Images/answer" + this.transform.gameObject.GetComponent<AnswerObject>().answerID + ".png"))
            {
                string path = Application.persistentDataPath + "/Images/answer" + this.transform.gameObject.GetComponent<AnswerObject>().answerID + ".png";
                cardArt.CrossFadeAlpha(1, 0, false);
                byte[] pngBytes = System.IO.File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(pngBytes);

                cardArt.texture = tex;
                cardText.text = "";

                pictureSet = true;
            }
            else
            {
                //Debug.Log(Application.persistentDataPath + "/Images/answer" + this.transform.gameObject.GetComponent<AnswerObject>().answerID + "         STILL LOADING");
            }
        }


        // Highlights the closest question drop area if it's close enough
        if (distanceFromClosestQuestionDropArea() < 5 && movingCards == true)
        {
            string hilite = closestQuestionDropAreaObject().GetComponent<QuestionObject>().questionText.text;

            foreach (GameObject go in QuestionDropsManager.questionDropGOs)
            {
                if (go.GetComponent<QuestionObject>().questionText.text == hilite)
                {
                    go.GetComponent<QuestionObject>().questionText.color = new Color(95f / 255f, 230f / 255f, 181f / 255f); ;
                }
                else
                    go.GetComponent<QuestionObject>().questionText.color = Color.grey;
            }
        }

        setPosition(currentHandNode);

        if (moveLeftFlag)
            moveLeft();

        if (moveRightFlag)
            moveRight();

        if (priorityView)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -9);
            this.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, currentHandNode * .1f);
            this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        location = this.transform.position;

        if (!movingCards)
            this.transform.position = Vector2.Lerp(location, destination, .05f);

        alreadyRan1 = false;
        alreadyRan2 = false;
    }

    // -------------------------------- //
    // Card Movement Functions
    // -------------------------------- //


    // sets the current position and the destination for the lerp move
    void setPosition(int handNode)
    {
        try
        {
            if (nodeNav == "hand" && handNode != -1)
                destination = HandPhysicsManager.handNodeGOs[handNode].transform.position;
        }
        catch
        {
            Debug.Log("(Can be ignored. I'm pretty sure) Error with handnode: " + handNode);
        }

        if (nodeNav == "question")
            destination = this.transform.position;
    }

    // Moves this card to the right
    void moveRight()
    {
        currentHandNode++;
        moveRightFlag = false;
    }

    // Moves this card to the left
    void moveLeft()
    {
        currentHandNode--;
        moveLeftFlag = false;
    }


    // -------------------------------- //
    // Mouse Functions
    // -------------------------------- //


    void OnMouseEnter()
    {
        if (HandPhysicsManager.cardIsGrabbed == false && HandPhysicsManager.cardHasPriority == false)
        {
            priorityView = true;
            HandPhysicsManager.cardHasPriority = true;
        }
    }

    void OnMouseDown()
    {
        //CollectionManager.printCollection();

        HandPhysicsManager.cardIsGrabbed = true;
        priorityView = true;

        movingCards = true;
        dragPlane = new Plane(myMainCamera.transform.forward, transform.position);
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);
        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);

        originalNode = currentHandNode;
    }

    void OnMouseDrag()
    {
        priorityView = true;
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);
        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        transform.position = camRay.GetPoint(planeDist);
        movingCards = true;

        // alreadyRan is to make sure that if a card causes other cards to move, it only causes the movement once.
        if (!alreadyRan1 && distanceFromCurrentNode() > 4 && distanceFromClosestNode() < 20 && HandPhysicsManager.handNodeGOs.Length != openNode && HandPhysicsManager.handNodeGOs.Length != currentHandNode - 1)
        {
            openNode = currentHandNode;
            currentHandNode = closestNode();

            if (openNode < currentHandNode)
            {
                HandPhysicsManager.moveAllCardsLeftBetween(openNode, currentHandNode);
                currentHandNode++;
            }
            else if (openNode > currentHandNode)
            {
                HandPhysicsManager.moveAllCardsRightBetween(currentHandNode, openNode);
                currentHandNode--;
            }
            alreadyRan1 = true;
        }
    }

    void OnMouseUp()
    {
        priorityView = false;
        HandPhysicsManager.cardIsGrabbed = false;

        if (distanceFromClosestQuestionDropArea() < 5)
        {
            nodeNav = "question";
            answer answerOnCard = this.GetComponent<AnswerObject>().answer;
            question questionOnNode = closestQuestionDropAreaObject().GetComponent<QuestionObject>().question;

            // If the card dropped corectly answers the question:
            if (QnAManager.answerQuestion(closestQuestionDropArea(), this.GetComponent<AnswerObject>().answerID)) 
            {
                QnAManager.attemptedAnswers.Add(new attemptedAnswer(true, questionOnNode, answerOnCard));           // Adds the correct answer to the list of attempted answers
                //HandPhysicsManager.moveAllCardsLeftBetween(currentHandNode, HandPhysicsManager.handNodeGOs.Length); // Fix the card positions, removed since it auto draws after a ques is answerd                                                                             // Set the nav of tjhis card so it moves towards the center of the quesiton node
                gameObject.tag = "Untagged";                                                                        // Change the tag of this card so it is not picked up by findCards()
                DeckManager.answerGraveyard.Add(this.GetComponent<AnswerObject>().answer);
                QuestionDropsManager.deleteQuestionDropArea(closestQuestionDropArea());                             // Delete the quesiton node and move question nodes under the deleted node up 1
                QuestionDropsManager.findQuestionDropAreas();                                                       // Refind QDAs                     
                HandPhysicsManager.findCards();                                                                     // Refind cards
                HandPhysicsManager.drawBool = true;
                HandPhysicsManager.moveAllCardsLeftBetween(currentHandNode, HandPhysicsManager.currentPresentNodeCount); // Moves correct cards to the left as there will soon be one lest card and node
                GameManager.currentModule.addCorrect();                                                                     // Increments the player data for number of correct answers answered
                StartCoroutine(BackendHook.sendQuestionInfo(questionOnNode.getQuestionID(), answerOnCard.getAnswerID(), true));        // Reports to the backend that a question was answered right
                QnAManager.currentAnswers.Remove(answerOnCard);
                QnAManager.currentQuestions.Remove(questionOnNode);

                Destroy(gameObject);
            }
            else
            {
                closestQuestionDropAreaObject().GetComponent<QuestionObject>().redTimer = .5f;
                QnAManager.attemptedAnswers.Add(new attemptedAnswer(false, questionOnNode, answerOnCard));
                StartCoroutine(BackendHook.sendQuestionInfo(questionOnNode.getQuestionID(), answerOnCard.getAnswerID(), false));
                GameManager.currentModule.addWrong();
                nodeNav = "hand";
            }
        }

        movingCards = false;
        setPosition(currentHandNode);
    }

    void OnMouseExit()
    {
        if (HandPhysicsManager.cardIsGrabbed == false && HandPhysicsManager.cardHasPriority == true)
        {
            priorityView = false;
            HandPhysicsManager.cardHasPriority = false;
        }
    }



    // -------------------------------- //
    // Hand Node Functions
    // -------------------------------- //



    // Returns the distance from the current hand node
    public float distanceFromCurrentNode()
    {
        return distanceBetween(this.transform.position.x, this.transform.position.y, HandPhysicsManager.handNodeGOs[currentHandNode].transform.position.x, HandPhysicsManager.handNodeGOs[currentHandNode].transform.position.y);
    }

    // Returns the ID of the closest hand node
    public int closestNode()
    {
        Vector2 nodeCoords;
        float distance;
        float smallestDistance = 5000000;
        int closestNode = -2;
        foreach (GameObject nodeGO in HandPhysicsManager.handNodeGOs)
        {
            nodeCoords = nodeGO.transform.position;
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
            if (distance < smallestDistance)
            {
                closestNode = nodeGO.GetComponent<HandNodeID>().nodeID;
                smallestDistance = distance;
            }
        }

        return closestNode;
    }

    // Returns the distance from the closest hand node
    public float distanceFromClosestNode()
    {
        float smallestDistance = 50000;
        float distance;
        foreach (GameObject node in HandPhysicsManager.handNodeGOs)
        {
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, node.transform.position.x, node.transform.position.y);
            if (distance < smallestDistance)
                smallestDistance = distance;
        }
        return smallestDistance;
    }


    // -------------------------------- //
    // Question Node Functions
    // -------------------------------- //


    // returns the ID of the closest question drop area
    // Also the ID of the question that that drop area corresponds to
    public int closestQuestionDropArea()
    {
        Vector2 nodeCoords;
        float smallestDistance = 50000;
        float distance;
        int closestNode = -1; // Card ID must never be -1
        foreach (GameObject qdaGOs in QuestionDropsManager.questionDropGOs)
        {
            nodeCoords = qdaGOs.transform.position;
            //distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, this.transform.position.x, nodeCoords.y);
            if (distance < smallestDistance)
            {
                closestNode = qdaGOs.GetComponent<QuestionObject>().questionID;
                smallestDistance = distance;
            }
        }

        return closestNode;
    }



    public GameObject closestQuestionDropAreaObject()
    {
        Vector2 nodeCoords;
        float smallestDistance = 50000;
        float distance;
        GameObject closestNode = null; 
        foreach (GameObject qdaGOs in QuestionDropsManager.questionDropGOs)
        {
            nodeCoords = qdaGOs.transform.position;
            //distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, this.transform.position.x, nodeCoords.y);
            if (distance < smallestDistance)
            {
                closestNode = qdaGOs;
                smallestDistance = distance;
            }
        }
        return closestNode;
    }

    // Returns the distance to the closest question drop area
    public float distanceFromClosestQuestionDropArea()
    {
        Vector2 nodeCoords;
        float smallestDistance = 50000;
        float distance;
        int closestNode = -1; // Card ID must never be -1
        foreach (GameObject qdaGOs in QuestionDropsManager.questionDropGOs)
        {
            nodeCoords = qdaGOs.transform.position;
            //distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, this.transform.position.x, nodeCoords.y);
            if (distance < smallestDistance)
            {
                closestNode = qdaGOs.GetComponent<QuestionObject>().questionID;
                smallestDistance = distance;
            }
        }

        return smallestDistance;
    }


    // -------------------------------- //
    // OtherFunctions
    // -------------------------------- //


    // Returns the distance between two points
    public float distanceBetween(float x1, float y1, float x2, float y2)
    {
        return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
}

