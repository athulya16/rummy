using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rummy
{
    [Serializable]
    public class ReleasedDeck : IEquatable<ReleasedDeck>
    {
        public Vector2 position;

        public List<Card> ReleasedCards = new List<Card>();
        int NumberOfReleasedCards;

        public void SetCardValues(List<byte> values)
        {
            /* if (ReleasedCards.Count != values.Count)
             {
                 Debug.LogError($"Displaying cards count {ReleasedCards.Count} is not equal to card values count {values.Count} for recievedDeck");
                 return;
             }*/
          

            for (int index = 0; index < values.Count; index++)
            {
                Card card = ReleasedCards[index];
                card.SetCardValue(values[index]);
                card.SetDisplayingOrder(index);
               
            }
            Debug.Log("finished3");
        }
        public void ShowCardValues()
        {
            foreach (Card card in ReleasedCards)
            {
                card.SetFaceUp(true);
            }
        }

        public void ReceiveDisplayingCard(Card card)
        {
            ReleasedCards.Add(card);
             //card.OwnerId = PlayerId;
            NumberOfReleasedCards++;
        }

        public void RepositionDisplayingCards(CardAnimator cardAnimator)
        {
            NumberOfReleasedCards = 0;
            foreach (Card card in ReleasedCards)
            {
                NumberOfReleasedCards++;
                cardAnimator.AddCardAnimation(card, position);
            }
        }

        public void SendDisplayingCardToPlayer(Player receivingPlayer, CardAnimator cardAnimator, List<byte> cardValues, bool isLocalPlayer)
        {
           /* int playerDisplayingCardsCount = ReleasedCards.Count;

            if (playerDisplayingCardsCount < cardValues.Count)
            {
                Debug.LogError("Not enough displaying cards");
                return;
            }

            for (int index = 0; index < cardValues.Count; index++)
            {

                Card card = null;
                byte cardValue = cardValues[index];

                if (isLocalPlayer)
                {
                    foreach (Card c in ReleasedCards)
                    {
                        if (c.Rank == Card.GetRank(cardValue) && c.Suit == Card.GetSuit(cardValue))
                        {
                            card = c;
                            break;
                        }
                    }
                }
                else
                {
                    card = ReleasedCards[playerDisplayingCardsCount - 1 - index];
                    card.SetCardValue(cardValue);
                    card.SetFaceUp(true);
                }

                if (card != null)
                {
                    ReleasedCards.Remove(card);
                    receivingPlayer.ReceiveDisplayingCard(card);
                    cardAnimator.AddCardAnimation(card, receivingPlayer.NextCardPosition());
                    NumberOfReleasedCards--;
                }
                else
                {
                    Debug.LogError("Unable to find displaying card.");
                }
            }*/

            RepositionDisplayingCards(cardAnimator);
        }

        public bool Equals(ReleasedDeck other)
        {
            return true;
        }

    }
}

