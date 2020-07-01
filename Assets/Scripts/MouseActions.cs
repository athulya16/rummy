using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Rummy
{
    [Serializable]
    public class CardSelectedEvent : UnityEvent<Card>
    {
    }

    public class MouseActions : MonoBehaviour
    {
        public CardSelectedEvent ReleaseSelectedCard = new CardSelectedEvent();
        public Transform cardDeck;
        public Transform releasedDeck;
        public GameObject card;
        public Game game;

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Card card = MouseOverCard();

                if (card != null)
                {
                    ReleaseSelectedCard.Invoke(card);
                }
            }
        }

        Card MouseOverCard()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (CheckIfCardDeckIsClicked(hit) && game.gameState == Game.RummyGameStates.DrawCard)
                {
                    //draw card from deck
                    Debug.Log("inside deck");
                    game.DrawCardFromCardDeck();
                }
                else if (CheckIfReleasedDeckIsClicked(hit) && game.gameState == Game.RummyGameStates.DrawCard)
                {
                    //draw card from released deck
                    Debug.Log("inside released deck");
                    game.DrawCardFromReleasedDeck();
                }
                else if (game.gameState == Game.RummyGameStates.ReleaseCard)
                {
                    Debug.Log("out");
                    Card card = hit.transform.gameObject.GetComponent<Card>();
                    if (card != null)
                    {
                        return card;
                    }
                }
            }
            return null;
        }

        bool CheckIfCardDeckIsClicked(RaycastHit2D eventData)
        {
            bool isCardDeck = false;

            float x = cardDeck.transform.position.x;
            float y = cardDeck.transform.position.y;
            RectTransform rt = (RectTransform)card.transform;
            float width = rt.rect.width;
            float height = rt.rect.height;

            if ((eventData.transform.position.x > x - width / 2 && eventData.transform.position.x < x + width / 2) && (eventData.transform.position.y < y + height / 2 && eventData.transform.position.y > y - height / 2))
            {
                isCardDeck = true;
            }
            return isCardDeck;

        }

        bool CheckIfReleasedDeckIsClicked(RaycastHit2D eventData)
        {
            bool isReleasedDeck = false;

            float x = releasedDeck.transform.position.x;
            float y = releasedDeck.transform.position.y;
            RectTransform rt = (RectTransform)card.transform;
            float width = rt.rect.width;
            float height = rt.rect.height;

            if ((eventData.transform.position.x > x - width / 2 && eventData.transform.position.x < x + width / 2) && (eventData.transform.position.y < y + height / 2 && eventData.transform.position.y > y - height / 2))
            {
                isReleasedDeck = true;
            }
            return isReleasedDeck;

        }
    }
}
