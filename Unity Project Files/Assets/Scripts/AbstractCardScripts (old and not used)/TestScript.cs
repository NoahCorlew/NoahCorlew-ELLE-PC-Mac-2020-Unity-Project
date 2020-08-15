using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public bool AddCardToCollection;
    public bool CreateDeck;
    public bool Shuffle;

    public bool DrawCard;
    public int CardsToDraw = 1;
    
    public bool DiscardCard;
    public int CardToDiscard;

    public bool ViewTopCards;
    public int CardsToView = 1;

    public bool ExileCard;
    public int CardToExile;

    public bool PlaceCardTop;
    public int CardToPlaceTop;

    public bool PlaceCardBottom;
    public int CardToPlaceBottom;

    public bool ShowDeck;
    public bool ShowGraveyard;
    public bool ViewHand;

    void Update()
    {
        if (AddCardToCollection)
        {
            CollectionProps.createCard(1, "one");
            CollectionProps.createCard(2, "two");
            CollectionProps.createCard(3, "three");
            CollectionProps.createCard(4, "four");
            CollectionProps.createCard(5, "five");
            CollectionProps.createCard(6, "six");
            CollectionProps.createCard(7, "seven");
            AddCardToCollection = false;
        }
        if (CreateDeck)
        {
            DeckProps.createDeck();
            CreateDeck = false;
        }
        if (Shuffle)
        {
            DeckProps.shuffle();
            Shuffle = false;
        }
        if (DrawCard)
        {
            HandProps.draw(CardsToDraw);
            DrawCard = false;
        }
        if (DiscardCard)
        {
            HandProps.discard(CardToDiscard);
            DiscardCard = false;
        }
        if (ExileCard)
        {
            HandProps.exile(CardToExile);
            ExileCard = false;
        }
        if (ViewTopCards)
        {
            DeckProps.showTop(CardsToView);
            ViewTopCards = false;
        }
        if (ShowDeck)
        {
            DeckProps.showDeck();
            ShowDeck = false;
        }
        if (ShowGraveyard)
        {
            DeckProps.showGraveyard();
            ShowGraveyard = false;
        }
        if (ViewHand)
        {
            HandProps.printHand();
            ViewHand = false;
        }
        if (PlaceCardBottom)
        {
            HandProps.placeBottom(CardToPlaceBottom);
            PlaceCardBottom = false;
        }
        if (PlaceCardTop)
        {
            HandProps.placeTop(CardToPlaceTop);
            PlaceCardBottom = false;
        }

    }
}
