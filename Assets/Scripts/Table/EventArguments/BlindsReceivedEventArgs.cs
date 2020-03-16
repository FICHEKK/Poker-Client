using System;

namespace Table.EventArguments
{
    public class BlindsReceivedEventArgs : EventArgs
    {
        public int DealerButtonIndex { get; }
        public int SmallBlindIndex { get; }
        public int BigBlindIndex { get; }

        public BlindsReceivedEventArgs(int dealerButtonIndex, int smallBlindIndex, int bigBlindIndex)
        {
            DealerButtonIndex = dealerButtonIndex;
            SmallBlindIndex = smallBlindIndex;
            BigBlindIndex = bigBlindIndex;
        }
    }
}