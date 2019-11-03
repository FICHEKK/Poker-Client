using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Hand : IComparable<Hand> {

    // An array of cards that this hand holds.
    public Card[] Cards { get; } = new Card[5];
    
    // Stores the data about this hand.
    public HandAnalyser HandAnalyser { get; }

    public Hand(Card c0, Card c1, Card c2, Card c3, Card c4) {
        Cards[0] = c0;
        Cards[1] = c1;
        Cards[2] = c2;
        Cards[3] = c3;
        Cards[4] = c4;
        
        Array.Sort(Cards);
        
        HandAnalyser = new HandAnalyser(this);
    }

    public int CompareTo(Hand other) {
        HandValue handValue = HandAnalyser.HandValue;
        HandValue handValueOther = other.HandAnalyser.HandValue;

        if ((int) handValue > (int) handValueOther) return 1;
        if ((int) handValue < (int) handValueOther) return -1;

        if (handValue == HandValue.HighCard) return CompareHighCard(other);
        if (handValue == HandValue.OnePair) return CompareOnePair(other);
        if (handValue == HandValue.TwoPair) return CompareTwoPair(other);
        if (handValue == HandValue.Threes) return CompareThrees(other);
        if (handValue == HandValue.Straight) return CompareStraight(other);
        if (handValue == HandValue.Flush) return CompareFlush(other);
        if (handValue == HandValue.FullHouse) return CompareFullHouse(other);
        if (handValue == HandValue.Fours) return CompareFours(other);
        if (handValue == HandValue.StraightFlush) return CompareStraightFlush(other);

        return 0;
    }
    
    private int CompareHighCard(Hand other) {
        List<int> kickers = new List<int>();
        List<int> kickersOther = new List<int>();

        for (int i = 0; i < 5; i++) {
            kickers.Add((int) Cards[i].Rank);
            kickersOther.Add((int) other.Cards[i].Rank);
        }

        return CompareListsOfRanks(kickers, kickersOther);
    }
    
    private int CompareOnePair(Hand other) {
        int pair = FindIndexOfRank(this, 2);
        int pairOther = FindIndexOfRank(other, 2);

        int result = CompareRanks(pair, pairOther);
        if (result != 0) return result;

        List<int> kickers = FindAllIndexesOfRank(this, 1);
        List<int> kickersOther = FindAllIndexesOfRank(other, 1);

        return CompareListsOfRanks(kickers, kickersOther);
    }
    
    private int CompareTwoPair(Hand other) {
        List<int> pairs = FindAllIndexesOfRank(this, 2);
        List<int> pairsOther = FindAllIndexesOfRank(other, 2);

        int result = CompareListsOfRanks(pairs, pairsOther);
        if (result != 0) return result;

        int kicker = FindIndexOfRank(this, 1);
        int kickerOther = FindIndexOfRank(this, 1);
        
        return CompareRanks(kicker, kickerOther);
    }
    
    private int CompareThrees(Hand other) {
        int threes = FindIndexOfRank(this, 3);
        int threesOther = FindIndexOfRank(other, 3);

        int result = CompareRanks(threes, threesOther);
        if (result != 0) return result;

        List<int> kickers = FindAllIndexesOfRank(this, 1);
        List<int> kickersOther = FindAllIndexesOfRank(other, 1);

        return CompareListsOfRanks(kickers, kickersOther);
    }
    
    private int CompareStraight(Hand other) {
        int highestRank = (int) Cards.Max().Rank;
        int highestRankOther = (int) other.Cards.Max().Rank;

        return CompareRanks(highestRank, highestRankOther);
    }
    
    private int CompareFlush(Hand other) {
        return CompareHighCard(other);
    }
    
    private int CompareFullHouse(Hand other) {
        int threes = FindIndexOfRank(this, 3);
        int threesOther = FindIndexOfRank(other, 3);

        int result = CompareRanks(threes, threesOther);
        if (result != 0) return result;
        
        int pair = FindIndexOfRank(this, 2);
        int pairOther = FindIndexOfRank(other, 2);

        return CompareRanks(pair, pairOther);
    }
    
    private int CompareFours(Hand other) {
        int fours = FindIndexOfRank(this, 4);
        int foursOther = FindIndexOfRank(other, 4);
        
        int result = CompareRanks(fours, foursOther);
        if (result != 0) return result;

        int kicker = FindIndexOfRank(this, 1);
        int kickerOther = FindIndexOfRank(other, 1);
        
        return CompareRanks(kicker, kickerOther);
    }
    
    private int CompareStraightFlush(Hand other) {
        return CompareStraight(other);
    }

    private static int FindIndexOfRank(Hand hand, int cardinality) {
        int[] rankCounters = hand.HandAnalyser.RankCounters;
        for (int i = 0; i < rankCounters.Length; i++) {
            if (rankCounters[i] == cardinality) {
                return i;
            }
        }

        return -1;
    }

    private static List<int> FindAllIndexesOfRank(Hand hand, int cardinality) {
        List<int> indexes = new List<int>();
        
        int[] rankCounters = hand.HandAnalyser.RankCounters;
        for (int i = 0; i < rankCounters.Length; i++) {
            if (rankCounters[i] == cardinality) {
                indexes.Add(i);
            }
        }

        return indexes;
    }

    private static int CompareRanks(int rank, int rankOther) {
        if (rank > rankOther) return 1;
        if (rank == rankOther) return 0;
        return -1;
    }

    private static int CompareListsOfRanks(List<int> ranks, List<int> ranksOther) {
        ranks.Sort();
        ranksOther.Sort();
        
        for (int i = ranks.Count - 1; i >= 0; i--) {
            if(ranks[i] > ranksOther[i]) return 1;
            if(ranks[i] < ranksOther[i]) return -1;
        }

        return 0;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 5; i++) {
            sb.Append(Cards[i]).Append(" ");
        }

        sb.Append(HandAnalyser.HandValue);

        return sb.ToString();
    }
}