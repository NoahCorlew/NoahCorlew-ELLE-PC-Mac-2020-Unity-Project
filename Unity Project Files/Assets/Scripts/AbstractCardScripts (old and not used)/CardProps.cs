using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProps : MonoBehaviour
{
    int pos;
    int handSize;
    public void setHandPosition(int pos, int handSize)
    {
        this.pos = pos;
    }

    public int getHandPosition()
    {
        return pos;
    }
}