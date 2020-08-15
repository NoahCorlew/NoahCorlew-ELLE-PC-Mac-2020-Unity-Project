using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionProps : MonoBehaviour
{
    public static Dictionary<int, Card> collectionContents = new Dictionary<int, Card>();

    public static void createCard(int termID, string text)
    {
        collectionContents.Add(termID, new Card(termID, text));
        Debug.Log("Card with text " + text + " and ID " + termID + " created and added to collection");
    }
}
