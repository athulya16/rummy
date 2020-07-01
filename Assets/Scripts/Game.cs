using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity;
using UnityEngine.UI;

namespace Rummy
{
    public class Game : MonoBehaviour
    {
        public Text MessageText;

        CardAnimator cardAnimator;

        public GameDataManager gameDataManager;

        public List<Transform> PlayerPositions = new List<Transform>();
        public List<Transform> BookPositions = new List<Transform>();
        public Transform releasedDeckPos;

        public Button dealButton;

        Player localPlayer;
        Player remotePlayer;
        ReleasedDeck releasedDeck;

        Player currentTurnPlayer;
        Player currentTurnTargetPlayer;

        Card selectedCard;
        Ranks selectedRank;
        bool isAnimating = false;

        public enum GameState
        {
            Idel,
            GameStarted,
            TurnStarted,
            TurnSelectingNumber,
            TurnConfirmedSelectedNumber,
            TurnWaitingForOpponentConfirmation,
            TurnOpponentConfirmed,
            TurnGoFish,
            GameFinished
        };

        public enum RummyGameStates
        {
            Idle,
            GameStarted,
            TurnStarted,
            DrawCard,
            MeldCard,
            SelectMeldCards,
            ReleaseCard,
            TurnOpponentConfirmed,
            WaitingForOpponentDrawCard,
            TurnOppnentReleaseCard,
            GameOver
        }


        public RummyGameStates gameState = RummyGameStates.Idle;

        private void Awake()
        {
            localPlayer = new Player();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";
            localPlayer.Position = PlayerPositions[0].position;
            localPlayer.BookPosition = BookPositions[0].position;

            remotePlayer = new Player();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.Position = PlayerPositions[1].position;
            remotePlayer.BookPosition = BookPositions[1].position;
            remotePlayer.IsAI = true;

            releasedDeck = new ReleasedDeck();
            releasedDeck.position = releasedDeckPos.position;

            cardAnimator = FindObjectOfType<CardAnimator>();
        }

        void Start()
        {
            gameState = RummyGameStates.GameStarted;
            GameFlow();
        }

        //****************** Game Flow *********************//
        /*       public void GameFlow()
               {
                   if (gameState > GameState.GameStarted)
                   {
                       CheckPlayersBooks();
                       ShowAndHidePlayersDisplayingCards();

                       if (gameDataManager.GameFinished())
                       {
                           gameState = GameState.GameFinished;
                       }
                   }

                   switch (gameState)
                   {
                       case GameState.Idel:
                           {
                               Debug.Log("IDEL");
                               break;
                           }
                       case GameState.GameStarted:
                           {
                               Debug.Log("GameStarted");
                               OnGameStarted();
                               break;
                           }
                       case GameState.TurnStarted:
                           {
                               Debug.Log("TurnStarted");
                               OnTurnStarted();
                               break;
                           }
                       case GameState.TurnSelectingNumber:
                           {
                               Debug.Log("TurnSelectingNumber");
                               OnTurnSelectingNumber();
                               break;
                           }
                       case GameState.TurnConfirmedSelectedNumber:
                           {
                               Debug.Log("TurnComfirmedSelectedNumber");
                               OnTurnConfirmedSelectedNumber();
                               break;
                           }
                       case GameState.TurnWaitingForOpponentConfirmation:
                           {
                               Debug.Log("TurnWaitingForOpponentConfirmation");
                               OnTurnWaitingForOpponentConfirmation();
                               break;
                           }
                       case GameState.TurnOpponentConfirmed:
                           {
                               Debug.Log("TurnOpponentConfirmed");
                               OnTurnOpponentConfirmed();
                               break;
                           }
                       case GameState.TurnGoFish:
                           {
                               Debug.Log("TurnGoFish");
                               OnTurnGoFish();
                               break;
                           }
                       case GameState.GameFinished:
                           {
                               Debug.Log("GameFinished");
                               OnGameFinished();
                               break;
                           }
                   }
               }*/

        public void GameFlow()
        {
            if (gameState > RummyGameStates.GameStarted)
            {
                CheckPlayersBooks();
                ShowAndHidePlayersDisplayingCards();

                if (gameDataManager.GameFinished())
                {
                    gameState = RummyGameStates.GameOver;
                }
            }

            switch (gameState)
            {
                case RummyGameStates.Idle:
                        Debug.Log("IDEL");
                        break;
                case RummyGameStates.GameStarted:
                        Debug.Log("GameStarted");
                        OnGameStarted();
                        break;
                case RummyGameStates.TurnStarted:
                        Debug.Log("TurnStarted");
                        OnTurnStarted();
                        break;
                case RummyGameStates.DrawCard:
                    Debug.Log("DrawCard");
                    OnWaitForDrawStarted();
                    break;
                case RummyGameStates.TurnOpponentConfirmed:
                    Debug.Log("TurnOpponentConfirmed");
                    OnTurnOpponentConfirmed();
                    break;
                case RummyGameStates.GameOver:
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
            }
        }

        void OnGameStarted()
        {
            /*gameDataManager = new GameDataManager(localPlayer, remotePlayer);
            gameDataManager.Shuffle();
            gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            gameState = GameState.TurnStarted;*/

            dealButton.gameObject.SetActive(true);
            cardAnimator.InitializeDeck();
            SetMessage($"Click DEAL to start game");
        }

        public void OnDealButtonClicked()
        {
            SetMessage($"Wait for your turn");
            dealButton.gameObject.SetActive(false);
            gameDataManager = new GameDataManager(localPlayer, remotePlayer);
            gameDataManager.Shuffle();
            gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardToReleasedDeck(1);

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealToReleasedDeck(releasedDeck, 1);
            //releasedDeck.ShowCardValues();

            gameState = RummyGameStates.TurnStarted;
        }

        void OnTurnStarted()
        {
            SwitchTurn();
            gameState = RummyGameStates.DrawCard;
            GameFlow();
        }

        void OnWaitForDrawStarted()
        {
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Your turn.Draw a card from the deck or pile.");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName}'s turn to draw");
                //Add logic for auto draw
            }
        }

        public void DrawCardFromCardDeck()
        {
            byte cardValue = gameDataManager.DrawCardValue();
            cardAnimator.DrawDisplayingCard(currentTurnPlayer, cardValue);
            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);
            gameState = RummyGameStates.ReleaseCard;
        }

        public void DrawCardFromReleasedDeck()
        {
            byte cardValue = gameDataManager.DrawFromReleasedDeck();
            cardAnimator.DrawReleasedCard(currentTurnPlayer, releasedDeck, cardValue);
            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);
            gameState = RummyGameStates.ReleaseCard;
        }

        /*void OnTurnStarted()
        {
            SwitchTurn();
            //gameState = GameState.TurnSelectingNumber;
            GameFlow();
        }*/

        public void OnTurnSelectingNumber()
        {
            ResetSelectedCard();

            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Your turn. Pick a card from your hand.");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName}'s turn");
            }

            if (currentTurnPlayer.IsAI)
            {
                selectedRank = gameDataManager.SelectRandomRanksFromPlayersCardValues(currentTurnPlayer);
                //gameState = GameState.TurnConfirmedSelectedNumber;
                GameFlow();
            }
        }

        public void OnTurnConfirmedSelectedNumber()
        {
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Asking {currentTurnTargetPlayer.PlayerName} for {selectedRank}s...");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName} is asking for {selectedRank}s...");
            }

            //gameState = GameState.TurnWaitingForOpponentConfirmation;
            GameFlow();
        }

        public void OnTurnWaitingForOpponentConfirmation()
        {
            if (currentTurnTargetPlayer.IsAI)
            {
                //gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }

        public void OnTurnOpponentConfirmed()
        {
        }

        public void OnTurnGoFish()
        {
            SetMessage($"Go fish!");

            byte cardValue = gameDataManager.DrawCardValue();

            if (cardValue == Constants.POOL_IS_EMPTY)
            {
                Debug.LogError("Pool is empty");
                return;
            }

            if (Card.GetRank(cardValue) == selectedRank)
            {
                cardAnimator.DrawDisplayingCard(currentTurnPlayer, cardValue);
            }
            else
            {
                cardAnimator.DrawDisplayingCard(currentTurnPlayer);
                //gameState = GameState.TurnStarted;
            }

            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);
        }

        public void OnGameFinished()
        {
            if (gameDataManager.Winner() == localPlayer)
            {
                SetMessage($"You WON!");
            }
            else
            {
                SetMessage($"You LOST!");
            }
        }

        //****************** Helper Methods *********************//
        public void ResetSelectedCard()
        {
            if (selectedCard != null)
            {
                selectedCard.OnSelected(false);
                selectedCard = null;
                selectedRank = 0;
            }
        }

        void SetMessage(string message)
        {
            MessageText.text = message;
        }

        public void SwitchTurn()
        {
            if (currentTurnPlayer == null)
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
                return;
            }

            if (currentTurnPlayer == localPlayer)
            {
                currentTurnPlayer = remotePlayer;
                currentTurnTargetPlayer = localPlayer;
            }
            else
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
            }
        }

        public void PlayerShowBooksIfNecessary(Player player)
        {
            Dictionary<Ranks, List<byte>> books = gameDataManager.GetBooks(player);

            if (books != null)
            {

                foreach (var book in books)
                {
                    player.ReceiveBook(book.Key, cardAnimator);

                    gameDataManager.RemoveCardValuesFromPlayer(player, book.Value);
                }

                gameDataManager.AddBooksForPlayer(player, books.Count);
            }

        }

        public void CheckPlayersBooks()
        {
            List<byte> playerCardValues = gameDataManager.PlayerCards(localPlayer);
            localPlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(localPlayer);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer);
            remotePlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(remotePlayer);

            List<byte> releasedCardValues = gameDataManager.ReleasedCards();
            releasedDeck.SetCardValues(releasedCardValues);
        }

        public void ShowAndHidePlayersDisplayingCards()
        {
            localPlayer.ShowCardValues();
            remotePlayer.HideCardValues();
            releasedDeck.ShowCardValues();
        }

        //****************** User Interaction *********************//
        public void ReleaseSelectedCard(Card card)
        {
            Card current = card;
            if(gameState == RummyGameStates.ReleaseCard)
            {
                byte cardValue = (byte)card.cardValue;
                gameDataManager.ReleaseCardToReleasedDeck(cardValue, currentTurnPlayer);
                cardAnimator.MoveToReleasedDeck(currentTurnPlayer, releasedDeck, cardValue, current);

                gameState = RummyGameStates.TurnStarted;
            }
        }

        public void OnOkSelected()
        {
           /* if (gameState == GameState.TurnSelectingNumber && localPlayer == currentTurnPlayer)
            {
                if (selectedCard != null)
                {
                    gameState = GameState.TurnConfirmedSelectedNumber;
                    GameFlow();
                }
            }
            else if (gameState == GameState.TurnWaitingForOpponentConfirmation && localPlayer == currentTurnTargetPlayer)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }*/
        }

        //****************** Animator Event *********************//
        public void AllAnimationsFinished()
        {
                GameFlow();

        }
    }
}
