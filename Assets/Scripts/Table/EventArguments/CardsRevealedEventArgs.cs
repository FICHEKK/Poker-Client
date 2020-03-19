using System;
using System.Collections.Generic;

namespace Table.EventArguments
{
    public class CardsRevealedEventArgs : EventArgs
    {
        public List<int> Indexes { get; }
        public List<string> FirstCards { get; }
        public List<string> SecondCards { get; }

        public CardsRevealedEventArgs(List<int> indexes, List<string> firstCards, List<string> secondCards)
        {
            Indexes = indexes;
            FirstCards = firstCards;
            SecondCards = secondCards;
        }
    }
}