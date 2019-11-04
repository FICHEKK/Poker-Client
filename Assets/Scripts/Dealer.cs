using UnityEngine;

public class Dealer : MonoBehaviour {
    void Start() {
        for (int i = 0; i < 20; i++) {
            Deck.Shuffle();

            Card c0 = Deck.GetNextCard();
            Card c1 = Deck.GetNextCard();
            Card c2 = Deck.GetNextCard();
            Card c3 = Deck.GetNextCard();
            Card c4 = Deck.GetNextCard();
            Card c5 = Deck.GetNextCard();
            Card c6 = Deck.GetNextCard();
            
            SevenCardEvaluator evaluator = new SevenCardEvaluator(c0, c1, c2, c3, c4, c5, c6);
            Debug.Log(c0 + " " + c1 + " " + c2  + " " + c3  + " " + c4  + " " + c5  + " " + c6 + " -> " + evaluator.BestHand);
        }
    }
}
