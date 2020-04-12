using System;
using System.Collections.Generic;

namespace Table.EventArguments
{
    public class ShowdownEventArgs : EventArgs
    {
        public List<Pot> SidePots { get; }

        public ShowdownEventArgs(List<Pot> sidePots)
        {
            SidePots = sidePots;
        }

        public class Pot
        {
            public int Value { get; }
            public List<int> WinnerIndexes { get; }
            public string BestHand { get; }

            public Pot(int value, List<int> winnerIndexes, string bestHand)
            {
                Value = value;
                WinnerIndexes = winnerIndexes;
                BestHand = bestHand;
            }
        }
    }
}