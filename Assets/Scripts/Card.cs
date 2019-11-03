using System;
using System.Text;

public class Card : IComparable<Card> {
    
    public Rank Rank { get; }
    public Suit Suit { get; }

    public Card(Rank rank, Suit suit) {
        Rank = rank;
        Suit = suit;
    }

    public int CompareTo(Card other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        if ((int) Rank < (int) other.Rank) return -1;
        if (Rank == other.Rank) return 0;
        return 1;
    }
    
    public override string ToString() {
        StringBuilder sb = new StringBuilder();

        switch (Rank) {
            case Rank.Two:   sb.Append("2"); break;
            case Rank.Three: sb.Append("3"); break;
            case Rank.Four:  sb.Append("4"); break;
            case Rank.Five:  sb.Append("5"); break;
            case Rank.Six:   sb.Append("6"); break;
            case Rank.Seven: sb.Append("7"); break;
            case Rank.Eight: sb.Append("8"); break;
            case Rank.Nine:  sb.Append("9"); break;
            case Rank.Ten:   sb.Append("10"); break;
            case Rank.Jack:  sb.Append("J"); break;
            case Rank.Queen: sb.Append("Q"); break;
            case Rank.King:  sb.Append("K"); break;
            case Rank.Ace:   sb.Append("A"); break;
        }

        switch (Suit) {
            case Suit.Heart:   sb.Append("♥"); break;
            case Suit.Diamond: sb.Append("♦"); break;
            case Suit.Spade:   sb.Append("♠"); break;
            case Suit.Club:    sb.Append("♣"); break;
        }

        return sb.ToString();
    }
}
