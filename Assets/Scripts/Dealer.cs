using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        List<Hand> hands = new List<Hand>();
            
        for (int i = 0; i < 1000; i++) {
            Deck.Shuffle();
            
            Card c0 = Deck.GetNextCard();
            Card c1 = Deck.GetNextCard();
            Card c2 = Deck.GetNextCard();
            Card c3 = Deck.GetNextCard();
            Card c4 = Deck.GetNextCard();
            
            hands.Add(new Hand(c0, c1, c2, c3, c4));
        }

        hands.Sort();
        hands.ForEach(hand => Debug.Log(hand));
    }
}
