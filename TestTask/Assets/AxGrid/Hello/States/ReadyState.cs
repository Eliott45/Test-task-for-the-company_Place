using System.Collections.Generic;
using AxGrid.Base;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

namespace AxGrid.Hello.States
{
    [State("Ready")]
    public class ReadyState : FSMState
    {
        /// <summary>
        /// Possible cards in the game.
        /// </summary>
        private static readonly List<GameObject> PrefabsCards = CardsKit();
        /// <summary>
        /// Deck of cards in the game.
        /// </summary>
        private static List<GameObject> _cards;
        private static Transform _collectionAnchorA;
        private Transform _collectionAnchorB;

        [Enter]
        public void Enter()
        {
            CreateAnchors();
            _cards = Settings.Model.GetList<GameObject>("CardsA");
            CreateCards();
            Hand();
        }

        private void CreateAnchors()
        {
            var goA = new GameObject("collectionA");
            var goB = new GameObject("collectionB");
            goA.transform.position = new Vector3(0,-3,100);
            goB.transform.position = new Vector3(0,3, 100);
            _collectionAnchorA = goA.transform;
            _collectionAnchorB = goB.transform;
        }

        private static void CreateCards()
        {
            // Populate collection with random cards from prefabs of card.
            for (var i = 0; i < Settings.Model.GetInt("CardCounterValue"); i++)
            {
                var go = Object.Instantiate(PrefabsCards[Random.Range(0, PrefabsCards.Count)], _collectionAnchorA, false);
                _cards.Add(go);
                // Debug.Log(Settings.Model.GetList<GameObject>("Cards")[i]);
            }
        }

        /// <summary>
        /// List of all possible cards in the game.
        /// </summary>
        /// <returns>Deck of Possible Cards.</returns>
        private static List<GameObject> CardsKit()
        {
            var cards = new List<GameObject>
            {
                Load("BronzeCard"),   
                Load("SilverCard"),
                Load("GoldCard"),
            };
            return cards;
        }

        /// <summary>
        /// Load game object from folder "Resources". 
        /// </summary>
        /// <param name="name">Name of game object.</param>
        /// <returns>Game object from "Resources".</returns>
        private static GameObject Load(string name)
        {
            return Resources.Load(name) as GameObject;
        }

        [Bind]
        public void OnBtn(string name)
        {
            switch (name)
            {
                case "Inc":
                    Settings.Model.Inc("CardCounterValue");
                    AddCard();
                    break;
                case "Dec":
                    if (Settings.Model.GetInt("CardCounterValue", 0) > 0)
                    {
                        Settings.Model.Dec("CardCounterValue");
                        RemoveCard();
                    }
                    break;
            }
        }

        private static void Hand()
        {
            int r = 0, l = 0;
            for (var i = 1; i < _cards.Count; i++)
            {
                var pos = _cards[i].transform.position;
                pos.x = 0;
                if (i % 2 == 0)
                {
                    pos += new Vector3(1 + r, 0, 0);
                    r++;
                }
                else
                {
                    pos += new Vector3(-1 - l, 0, 0);
                    l++;
                }

                _cards[i].GetComponent<Card>().SetLayer(-i);
                _cards[i].transform.position = pos;
            }
        }
        
        private static void AddCard()
        {
            _cards.Add(Object.Instantiate(PrefabsCards[Random.Range(0, PrefabsCards.Count)], _collectionAnchorA, false));
            Hand();
        }
        
        private static void RemoveCard()
        {
            var lastElement = _cards[_cards.Count - 1];
            Object.Destroy(lastElement);
            _cards.Remove(lastElement);
            Hand();
        }
        
    }
}