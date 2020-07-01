﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rummy
{
    public class CardAnimation
    {
        public Card card;
        public Vector2 destination;
        public Quaternion rotation;
 
        public CardAnimation(Card c, Vector2 pos)
        {
            card = c;
            destination = pos;
            rotation = Quaternion.identity;
        }

        public CardAnimation(Card c, Vector2 pos, Quaternion rot)
        {
            card = c;
            destination = pos;
            rotation = rot;
        }

        public bool Play()
        {
            bool finished = false;

            if (Vector2.Distance(card.transform.position, destination) < Constants.CARD_SNAP_DISTANCE)
            {
                card.transform.position = destination;
                finished = true;
            }
            else
            {
                card.transform.position = Vector2.MoveTowards(card.transform.position, destination, Constants.CARD_MOVEMENT_SPEED * Time.deltaTime);
                card.transform.rotation = Quaternion.Lerp(card.transform.rotation, rotation, Constants.CARD_ROTATION_SPEED * Time.deltaTime);
            }

            return finished;
        }
    }

    /// <summary>
    /// Controls all card animations in the game
    /// </summary>
    public class CardAnimator : MonoBehaviour
    {
        public GameObject CardPrefab;

        public List<Card> DisplayingCards;
        public List<Card> releasedDeckCards;

        public Queue<CardAnimation> cardAnimations;

        CardAnimation currentCardAnimation;

        Vector2 startPosition = new Vector2(-5f, 1f);
        public Transform startPos;

        // invoked when all queued card animations have been played
        public UnityEvent OnAllAnimationsFinished = new UnityEvent();

        bool working = false;

        void Start()
        {
            cardAnimations = new Queue<CardAnimation>();
            //InitializeDeck();
        }

        public void InitializeDeck()
        {
            DisplayingCards = new List<Card>(); 
            releasedDeckCards = new List<Card>();

            for (byte value = 0; value < 52; value++)
            {
                // Vector2 newPosition = startPosition + Vector2.right * Constants.DECK_CARD_POSITION_OFFSET * value;
                Vector2 newPosition = new Vector2(startPos.transform.position.x , startPos.transform.position.y);
                GameObject newGameObject = Instantiate(CardPrefab, newPosition, Quaternion.identity);
                newGameObject.transform.parent = transform;
                Card card = newGameObject.GetComponent<Card>();
                card.SetDisplayingOrder(-1);
                card.transform.position = newPosition;
                DisplayingCards.Add(card);
            }
        }

        public void AddToReleasedDeck()
        {

        }

        public void DealDisplayingCards(Player player, int numberOfCard)
        {
            int start = DisplayingCards.Count - 1;
            int finish = DisplayingCards.Count - 1 - numberOfCard;

            List<Card> cardsToRemoveFromDeck = new List<Card>();

            for (int i = start; i > finish; i--)
            {
                Card card = DisplayingCards[i];
                player.ReceiveDisplayingCard(card);
                cardsToRemoveFromDeck.Add(card);
                AddCardAnimation(card, player.NextCardPosition());
            }

            foreach (Card card in cardsToRemoveFromDeck)
            {
                DisplayingCards.Remove(card);
            }
        }

        public void DealToReleasedDeck(ReleasedDeck releasedDeck, int numberOfCard)
        {
            int start = DisplayingCards.Count - 1;
            int finish = DisplayingCards.Count - 1 - numberOfCard;

            List<Card> cardsToRemoveFromDeck = new List<Card>();

            for (int i = start; i > finish; i--)
            {
                Card card = DisplayingCards[i];
                releasedDeck.ReceiveDisplayingCard(card);
                cardsToRemoveFromDeck.Add(card);
                AddCardAnimation(card, releasedDeck.position);
            }

            foreach (Card card in cardsToRemoveFromDeck)
            {
                DisplayingCards.Remove(card);
            }
        }

        public void DrawDisplayingCard(Player player)
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition());

                DisplayingCards.Remove(card);
            }
        }

        public void DrawDisplayingCard(Player player, byte value)
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                card.SetCardValue(value);
                card.SetFaceUp(true);
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition());

                DisplayingCards.Remove(card);
            }
        }

        public void DrawReleasedCard(Player player, ReleasedDeck releasedDeck, byte value)
        {
            int numberOfDisplayingCard = releasedDeck.ReleasedCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = releasedDeck.ReleasedCards[numberOfDisplayingCard - 1];
                card.SetCardValue(value);
                card.SetFaceUp(true);
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition());

                releasedDeckCards.Remove(card);
            }
        }
         
        public void MoveToReleasedDeck(Player player, ReleasedDeck releasedDeck, int cardValue, Card card)
        {
            Card cardToRemoveFromPlayer = null;
            player.RemoveFromDisplayingCard(card);
            releasedDeck.ReceiveDisplayingCard(card);
            cardToRemoveFromPlayer = card;
            AddCardAnimation(card, releasedDeck.position);

            cardToRemoveFromPlayer = null;
            player.RepositionDisplayingCards(this);


            /*
                        int start = DisplayingCards.Count - 1;
                        int finish = DisplayingCards.Count - 1 - numberOfCard;

                        List<Card> cardsToRemoveFromDeck = new List<Card>();

                        for (int i = start; i > finish; i--)
                        {
                            Card card = DisplayingCards[i];
                            releasedDeck.ReceiveDisplayingCard(card);
                            cardsToRemoveFromDeck.Add(card);
                            AddCardAnimation(card, releasedDeck.position);
                        }

                        foreach (Card card in cardsToRemoveFromDeck)
                        {
                            DisplayingCards.Remove(card);
                        }*/
        }

        public void AddCardAnimation(Card card, Vector2 position)
        {
            CardAnimation ca = new CardAnimation(card, position);
            cardAnimations.Enqueue(ca);
            working = true;
        }

        public void AddCardAnimation(Card card, Vector2 position, Quaternion rotation)
        {
            CardAnimation ca = new CardAnimation(card, position, rotation);
            cardAnimations.Enqueue(ca);
            working = true;
        }

        private void Update()
        {
            if (currentCardAnimation == null)
            {
                NextAnimation();
            }
            else
            {
                if (currentCardAnimation.Play())
                {
                    NextAnimation();
                }
            }
        }

        void NextAnimation()
        {
            currentCardAnimation = null;

            if (cardAnimations.Count > 0)
            {
                CardAnimation ca = cardAnimations.Dequeue();
                currentCardAnimation = ca;
            }
            else
            {
                if (working)
                {
                    working = false;
                    OnAllAnimationsFinished.Invoke();
                }
            }
        }
    }
}
