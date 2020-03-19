using System;
using System.Collections.Generic;

namespace Table.EventArguments
{
    public class BlindsReceivedEventArgs : EventArgs
    {
        public List<int> JustJoinedPlayerIndexes { get; }
        public int DealerButtonIndex { get; }
        public int SmallBlindIndex { get; }
        public int BigBlindIndex { get; }

        public BlindsReceivedEventArgs(List<int> justJoinedPlayerIndexes, int dealerButtonIndex, int smallBlindIndex, int bigBlindIndex)
        {
            JustJoinedPlayerIndexes = justJoinedPlayerIndexes;
            DealerButtonIndex = dealerButtonIndex;
            SmallBlindIndex = smallBlindIndex;
            BigBlindIndex = bigBlindIndex;
        }
    }
}