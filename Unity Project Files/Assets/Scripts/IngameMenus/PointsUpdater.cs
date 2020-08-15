using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsUpdater : MonoBehaviour
{
    public GameObject pointsDisplay;

    void Update()
    {
        pointsDisplay.GetComponent<Text>().text = "Points: " + QnAManager.points.ToString();
    }
}
