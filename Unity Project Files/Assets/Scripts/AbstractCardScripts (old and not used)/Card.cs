using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    string text;
    int cardID;

    public Card(int cardID, string text)
    {
        this.text = text;
        this.cardID = cardID;
    }

    public string getText()
    {
        return text;
    }

    public int getCardID()
    {
        return cardID;
    }
}
