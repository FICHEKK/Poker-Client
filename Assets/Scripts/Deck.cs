using System;
using Random = System.Random;

public static class Deck {

    private static int index;
    private static Random rng = new Random();
    private static Card[] cards = new Card[52];

    static Deck() {
        int i = 0;
        
        foreach (Suit suit in Enum.GetValues(typeof(Suit))) {
            foreach (Rank rank in Enum.GetValues(typeof(Rank))) {
                cards[i] = new Card(rank, suit);
                i++;
            }
        }
    }

    public static void Shuffle() {
        int n = cards.Length;
        
        while (n > 1)  {
            int k = rng.Next(n--);
            
            Card temp = cards[n];
            cards[n] = cards[k];
            cards[k] = temp;
        }

        index = 0;
    }

    public static bool HasNextCard() {
        return index < cards.Length;
    }

    public static Card GetNextCard() {
        return cards[index++];
    }
}