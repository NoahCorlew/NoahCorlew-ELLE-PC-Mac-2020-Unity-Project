using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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


    // Variables for click and drag system
    private Plane dragPlane;
    private Vector3 offset;
    private Camera myMainCamera;


    void Start()
    {
        nodeNav = "hand";
        myMainCamera = Camera.main;
        this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = this.GetComponent<AnswerObject>().answerID.ToString();
    }

    void FixedUpdate()
    {
        location = this.transform.position;

        if (!movingCards)
            setPosition(currentHandNode);

        this.transform.position = Vector2.Lerp(location, destination, .1f);

        if (moveLeftFlag)
            moveLeft();

        if (moveRightFlag)
            moveRight();

        if (!priorityView)
            GetComponent<SpriteRenderer>().sortingOrder = currentHandNode;

        movingCards = false;
        alreadyRan1 = false;
        alreadyRan2 = false;
    }


    // -------------------------------- //
    // Card Movement Functions
    // -------------------------------- //


    // sets the current position and the destination for the lerp move
    // TODO: When player drops card on questoin node, the handNode is set to be wrong after the first pass
    void setPosition(int handNode)
    {
        if (nodeNav == "hand")
            destination = HandPhysicsManager.handNodeGOs[handNode].transform.position;

        if (nodeNav == "question")
            destination = this.gameObject.transform.position;
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


    void OnMouseOver()
    {
        // Want to change this so that the view priority works kinda like a stack. Issue: Stack can have dup. items.
        foreach (GameObject card in HandPhysicsManager.cardGOs)
        {
            if (card.GetComponent<CardPhysics>().priorityView == true)
                card.GetComponent<CardPhysics>().priorityView = false;
        }

        priorityView = true;
        GetComponent<SpriteRenderer>().sortingOrder = 100;
        //this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().sortingOrder = 101;
    }

    void OnMouseDown()
    {
        movingCards = true;
        dragPlane = new Plane(myMainCamera.transform.forward, transform.position);
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);
        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        offset = transform.position - camRay.GetPoint(planeDist);

        originalNode = currentHandNode;
    }

    void OnMouseDrag()
    {
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);
        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        transform.position = camRay.GetPoint(planeDist) + offset;
        movingCards = true;

        // alreadyRan is to make sure that if a card causes other cards to move, it only causes the movement once.
        if (!alreadyRan1 && distanceFromCurrentNode() > 4 && distanceFromClosestNode() < 20)
        {
            openNode = currentHandNode;
            currentHandNode = closestNode();

            if (openNode <= currentHandNode)
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
        if (distanceFromClosestQuestionDropArea() < 4)
        {
            answer answerOnCard = this.GetComponent<AnswerObject>().answer;
            question questionOnNode = closestQuestionDropAreaObject().GetComponent<QuestionObject>().question;

            // If the card dropped corectly answers the question:
            if (QnAManager.answerQuestion(closestQuestionDropArea(), this.GetComponent<AnswerObject>().answerID)) 
            {
                QnAManager.attemptedAnswers.Add(new attemptedAnswer(true, questionOnNode, answerOnCard));           // Adds the correct answer to the list of attempted answers
                //HandPhysicsManager.moveAllCardsLeftBetween(currentHandNode, HandPhysicsManager.handNodeGOs.Length); // Fix the card positions, removed since it auto draws after a ques is answerd 
                nodeNav = "question";                                                                               // Set the nav of tjhis card so it moves towards the center of the quesiton node
                gameObject.tag = "Untagged";                                                                        // Change the tag of this card so it is not picked up by findCards()
                QuestionDropsManager.deleteQuestionDropArea(closestQuestionDropArea());                             // Delete the quesiton node and move question nodes under the deleted node up 1
                QuestionDropsManager.findQuestionDropAreas();                                                       // Refind QDAs                     
                HandPhysicsManager.findCards();                                                                     // Refind cards
                StartCoroutine(CoroutineFix());                                                                     // Move the hand nodes as there is now one more node than cards. See note.
                HandPhysicsManager.drawBool = true;
            }
            else
            {
                QnAManager.attemptedAnswers.Add(new attemptedAnswer(false, questionOnNode, answerOnCard));
            }
        }
        closestQuestionDropArea();
        setPosition(currentHandNode);
        movingCards = false;
    }

    void OnMouseExit()
    {
        priorityView = false;
    }

    /* When the player drops a card on an answer, all cards are moved left and the number of nodes is decreased. 
     * However, when this happens, the card on the leftmost node (which is on the node that is getting moved away from the hand and deactivated)
     * is still attatched to that soon to be removed node. This causes the card to for a very small amount of time move towards the removed node. 
     * This causes that card to move from offscreen to the new leftmost node, which does not look good. This coroutine adds a small delay between moving
     * the cards to the left and moving the leftmost node offscreen, allowing for any card on the leftmost node to move close enough to what will
     * soon be the leftmost node.
     * And I hate it.
     */
    IEnumerator CoroutineFix()
    {
        yield return new WaitForSeconds(.05f);
        HandPhysicsManager.moveNodes(HandPhysicsManager.currentPresentNodeCount - 1);
        Destroy(gameObject);
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
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
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
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
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
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, nodeCoords.x, nodeCoords.y);
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

