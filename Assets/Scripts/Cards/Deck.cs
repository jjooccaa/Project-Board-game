using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck<T>
{
    List<T> cards;
    List<T> discard;

    public Deck(List <T> cards)
    {
        this.cards = cards;
        discard = new List<T>();
    }

    // shuffle the current deck of cards
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; --i)
        {
            int j = Random.Range(0, i + 1);
            T card = cards[j];
            cards[j] = cards[i];
            cards[i] = card;
        }
    }
    // Return a list of drawn Cards from deck
    public List<T> Draw(int numberToDraw = 1)
    {
        if (cards.Count > 0)
        {
            if (numberToDraw > cards.Count)
                numberToDraw = cards.Count;

            List<T> drawnCards = new List<T>();

            for (int i = 0; i < numberToDraw; ++i)
            {
                drawnCards.Add(cards[0]);
                cards.RemoveAt(0);
            }

            return drawnCards;
        }

            return null;
    }
    public List<T> Cards()
    {
        return cards;
    }
    public void Graveyard(T card)
    {
        discard.Add(card);
    }
}
