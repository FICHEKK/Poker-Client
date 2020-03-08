using System;

namespace Table.EventArguments
{
    public class TableEmptyEventArgs : EventArgs
    {
        public int SeatIndex { get; }
        public int SmallBlind { get; }
        public int BuyIn { get; }

        public TableEmptyEventArgs(int seatIndex, int smallBlind, int buyIn)
        {
            SeatIndex = seatIndex;
            SmallBlind = smallBlind;
            BuyIn = buyIn;
        }
    }
}