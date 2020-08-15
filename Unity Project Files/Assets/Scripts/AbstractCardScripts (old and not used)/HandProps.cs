using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandProps : MonoBehaviour
{
    public static int maxHandSize = 7;
    public static List<Card> handContents = new List<Card>();

    public static void printHand()
    {
        foreach (Card item in handContents)
        {
            Debug.Log("Printing hand " + item.getCardID());
        }
    }

    public static void draw(int x)
    {
        if (x > DeckProps.deckContents.Count)
        {
            x = DeckProps.deckContents.Count;
            Debug.Log("Deck has not enough cards");
        }

        while (x != 0)
        {
            handContents.Add(DeckProps.deckContents[x - 1]);
            Debug.Log("Drawing card with ID " + DeckProps.deckContents[x - 1].getCardID());
            DeckProps.deckContents.RemoveAt(x - 1);
            x -= 1;
        }

        var random = new Random();
        while (handContents.Count > maxHandSize)
        {
            Debug.Log("Discarding random");
            discard(Random.Range(0, handContents.Count));
        }
    }

    public static void returnFromGraveyard(int cardID)
    {
        if (!DeckProps.graveyard.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Graveyard does not contain cardID " + cardID);
        else
        {
            handContents.Add(CollectionProps.collectionContents[cardID]);
            DeckProps.graveyard.Remove(CollectionProps.collectionContents[cardID]);
            Debug.Log("Card moved from graveyard to hand " + cardID); 
        }
    }

    public static void searchDeck(int cardID)
    {
        if (!DeckProps.deckContents.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Deck does not contain cardID " + cardID);
        else
        {
            handContents.Add(CollectionProps.collectionContents[cardID]);
            DeckProps.deckContents.Remove(CollectionProps.collectionContents[cardID]);
            Debug.Log("Card moved from Deck to hand " + cardID); 
        }
    }

    public static void discard(int cardID)
    {
        if (!handContents.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Hand does not contain cardID " + cardID);
        else
        {
            handContents.Remove(CollectionProps.collectionContents[cardID]);
            DeckProps.graveyard.Insert(0, CollectionProps.collectionContents[cardID]);
            Debug.Log("Discarding card with ID " + cardID);
        }
    }

    public static void exile(int cardID)
    {
        if (!handContents.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Hand does not contain cardID " + cardID);
        else
        {
            Debug.Log("Exilng card from hand " + cardID);
            handContents.Remove(CollectionProps.collectionContents[cardID]);
        }
    }

    public static void placeBottom(int cardID)
    {
        if (!handContents.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Hand does not contain cardID " + cardID);
        else
        {
            handContents.Remove(CollectionProps.collectionContents[cardID]);
            DeckProps.deckContents.Add(CollectionProps.collectionContents[cardID]);
            Debug.Log("Placing card on bottom of Deck with ID " + cardID);
        }
    }

    public static void placeTop(int cardID)
    {
        if (!handContents.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Hand does not contain cardID " + cardID);
        else
        {
            handContents.Remove(CollectionProps.collectionContents[cardID]);
            DeckProps.deckContents.Insert(0, CollectionProps.collectionContents[cardID]);
            Debug.Log("Placing card on top of Deck with ID " + cardID);
        }
    }

    public static void placeRandom(int cardID)
    {
        if (!handContents.Contains(CollectionProps.collectionContents[cardID]))
            Debug.Log("Hand does not contain cardID " + cardID);
        else
        {
            handContents.Remove(CollectionProps.collectionContents[cardID]);
            DeckProps.deckContents.Insert(Random.Range(0, DeckProps.getDeckSize()), CollectionProps.collectionContents[cardID]);
            Debug.Log("Placing card on top of Deck with ID " + cardID);
        }
    }
}
