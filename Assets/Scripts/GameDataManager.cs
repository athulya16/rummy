using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rummy
{
    [Serializable]
    public class GameDataManager
    {
        Player localPlayer;
        Player remotePlayer;

        [SerializeField]
        ProtectedData protectedData;

        public GameDataManager(Player local, Player remote)
        {
            localPlayer = local;
            remotePlayer = remote;
            protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId);
        }

        public void Shuffle()
        {
            List<byte> cardValues = new List<byte>();

            for (byte value = 0; value < 52; value++)
            {
                cardValues.Add(value);
            }

            List<byte> poolOfCards = new List<byte>();

            for (int index = 0; index < 52; index++)
            {
                int valueIndexToAdd = UnityEngine.Random.Range(0, cardValues.Count);

                byte valueToAdd = cardValues[valueIndexToAdd];
                poolOfCards.Add(valueToAdd);
                cardValues.Remove(valueToAdd);
            }

            protectedData.SetPoolOfCards(poolOfCards);
        }

        public void DealCardValuesToPlayer(Player player, int numberOfCards)
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;
            int start = numberOfCardsInThePool - 1 - numberOfCards;

            List<byte> cardValues = poolOfCards.GetRange(start, numberOfCards);
            poolOfCards.RemoveRange(start, numberOfCards);

            protectedData.AddCardValuesToPlayer(player, cardValues);
        }

        public void DealCardToReleasedDeck(int numberOfCards)
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();
            int numberOfCardsInThePool = poolOfCards.Count;
            int start = numberOfCardsInThePool - 1 - numberOfCards;
            byte cardValue = poolOfCards[numberOfCardsInThePool - 1];
            poolOfCards.Remove(cardValue);

            protectedData.AddCardValueToReleaseDeck(cardValue);
        }

        public void ReleaseCardToReleasedDeck(byte value, Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);
            protectedData.PlayerCards(player).Remove(value);

            protectedData.AddCardValueToReleaseDeck(value);

            
            Debug.Log(protectedData.PlayerCards(player));


            
            //protectedData.PlayerCards(player).Remove();
           /* protectedData.PlayerCards().Remove(cardValue);

            protectedData.AddCardValueToReleaseDeck(cardValues);*/
        }

        public byte DrawCardValue()
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;

            if (numberOfCardsInThePool > 0)
            {
                byte cardValue = poolOfCards[numberOfCardsInThePool - 1];
                poolOfCards.Remove(cardValue);

                return cardValue;
            }

            return Constants.POOL_IS_EMPTY;
        }

        public byte DrawFromReleasedDeck()
        {
            List<byte> releasedPool = protectedData.ReleasedCards();

            int numberOfCardsInThePool = releasedPool.Count;

            if (numberOfCardsInThePool > 0)
            {
                byte cardValue = releasedPool[numberOfCardsInThePool - 1];
                releasedPool.Remove(cardValue);
                protectedData.SetReleasedDeck(releasedPool);

                return cardValue;
            }

            return Constants.POOL_IS_EMPTY;
        }

        public List<byte> PlayerCards(Player player)
        {
            return protectedData.PlayerCards(player);
        }

        public List<byte> ReleasedCards()
        {
            return protectedData.ReleasedCards();
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            protectedData.AddCardValuesToPlayer(player, cardValues);
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            protectedData.AddCardValueToPlayer(player, cardValue);
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            protectedData.RemoveCardValuesFromPlayer(player, cardValuesToRemove);
        }

        public void AddBooksForPlayer(Player player, int numberOfNewBooks)
        {
            protectedData.AddBooksForPlayer(player, numberOfNewBooks);
        }

        public Player Winner()
        {
            string winnerPlayerId = protectedData.WinnerPlayerId();
            if (winnerPlayerId.Equals(localPlayer.PlayerId))
            {
                return localPlayer;
            }
            else
            {
                return remotePlayer;
            }
        }

        public bool GameFinished()
        {
            return protectedData.GameFinished();
        }

        public List<byte> TakeCardValuesWithRankFromPlayer(Player player, Ranks ranks)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);

            List<byte> result = new List<byte>();

            foreach (byte cv in playerCards)
            {
                Debug.Log(Card.GetRank(cv));
                if (Card.GetRank(cv) == ranks)
                {
                    result.Add(cv);
                }
            }

            protectedData.RemoveCardValuesFromPlayer(player, result);

            return result;
        }

        public Dictionary<Ranks, List<byte>> GetBooks(Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);

            var groups = playerCards.GroupBy(Card.GetRank).Where(g => g.Count() == 4);

            if (groups.Count() > 0)
            {
                Dictionary<Ranks, List<byte>> setOfFourDictionary = new Dictionary<Ranks, List<byte>>();

                foreach (var group in groups)
                {
                    List<byte> cardValues = new List<byte>();

                    foreach (var value in group)
                    {
                        cardValues.Add(value);
                    }

                    setOfFourDictionary[group.Key] = cardValues;
                }

                return setOfFourDictionary;
            }

            return null;
        }

        public Ranks SelectRandomRanksFromPlayersCardValues(Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);
            int index = UnityEngine.Random.Range(0, playerCards.Count);

            return Card.GetRank(playerCards[index]);
        }
    }
}
