using System;

namespace Table.EventArguments {
    public class TableEmptyEventArgs : EventArgs {
        public int SeatIndex { get; }
        public int BuyIn { get; }

        public TableEmptyEventArgs(int seatIndex, int buyIn) {
            SeatIndex = seatIndex;
            BuyIn = buyIn;
        }
    }
}