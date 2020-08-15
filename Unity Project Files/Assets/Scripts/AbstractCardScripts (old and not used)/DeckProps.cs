using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckProps : MonoBehaviour
{
    public static List<Card> deckContents = new List<Card>();
    public static List<Card> graveyard = new List<Card>();
    
    public static void createDeck()
    {
        Dictionary<int, Card>.ValueCollection values = CollectionProps.collectionContents.Values;
        foreach (Card item in values)
        {
            deckContents.Add(item);
            Debug.Log("Card with text " + item.getText() + " and ID " + item.getCardID() + " added to deck");
        }   
    }

    public static void shuffle()
    {
        List<Card> shuffle = new List<Card>();
        int size = deckContents.Count;
        int i;
        while(size != 0)
        {
            i = Random.Range(0,size);
            shuffle.Add(deckContents.ElementAt(i));
            deckContents.RemoveAt(i);
            size -= 1;
        }
        Debug.Log("Deck shuffled");
        deckContents = shuffle;
    }

    public static int getDeckSize()
    {
        return deckContents.Count;
    }

    public static int getGraveyardSize()
    {
        return graveyard.Count;
    }

    public static void showDeck()
    {
        Debug.Log("Printing Deck");
        foreach (Card card in deckContents)
        {
            Debug.Log(card.getCardID());
        }
        Debug.Log("Done");
    }

    public static void showGraveyard()
    {
        Debug.Log("Printing Graveyard");
        foreach (Card card in graveyard)
        {
            Debug.Log(card.getCardID());
        }
        Debug.Log("Done");
    }

    public static void showTop(int x)
    {
        if (x > deckContents.Count)
            x = deckContents.Count;

        int i = 0;
        while (x != i)
        {
            Debug.Log("Show top n " + deckContents[i].getCardID());
            i += 1;
        }
    }
}
