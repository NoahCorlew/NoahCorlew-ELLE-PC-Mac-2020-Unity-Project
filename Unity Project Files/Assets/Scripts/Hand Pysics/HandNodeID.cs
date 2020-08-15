using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This script is on the physical hand nodes, and is used by cards to access the ID of the closest node
public class HandNodeID : MonoBehaviour
{
    public int nodeID;
    public bool hasCard;

    /*
    void FixedUpdate()
    {
        if (!hasCard && distanceFromClosestCard() < .3)
            hasCard = true;
        else
            hasCard = false;
    }

    public float distanceFromClosestCard()
    {
        int nodePos = 0;
        float smallestDistance = 50000;
        float distance;
        foreach (GameObject node in HandPhysicsManager.cardGOs)
        {
            distance = distanceBetween(this.transform.position.x, this.transform.position.y, node.transform.position.x, node.transform.position.y);
            if (distance < smallestDistance)
                smallestDistance = distance;
        }

        return smallestDistance;
    }

    public float distanceBetween(float x1, float y1, float x2, float y2)
    {
        return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
    */

}
