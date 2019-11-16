using System;
using Random = System.Random;

public class Deck {

    private static readonly Random Rng = new Random();
    
    private int _index;
    private readonly Card[] _cards = new Card[52];

    public Deck() {
        int i = 0;
        
        foreach (Suit suit in Enum.GetValues(typeof(Suit))) {
            foreach (Rank rank in Enum.GetValues(typeof(Rank))) {
                _cards[i] = new Card(rank, suit);
                i++;
            }
        }
    }

    public void Shuffle() {
        int n = _cards.Length;
        
        while (n > 1)  {
            int k = Rng.Next(n--);
            
            Card temp = _cards[n];
            _cards[n] = _cards[k];
            _cards[k] = temp;
        }

        _index = 0;
    }

    public bool HasNextCard() {
        return _index < _cards.Length;
    }

    public Card GetNextCard() {
        return _cards[_index++];
    }
}