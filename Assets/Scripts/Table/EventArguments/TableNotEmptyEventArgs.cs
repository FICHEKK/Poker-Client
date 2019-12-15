using System;

namespace Table.EventArguments {
    public class TableNotEmptyEventArgs : EventArgs {
        public int SeatIndex { get; }
        public string Username { get; }
        public int ChipCount { get; }
        public int BuyIn { get; }

        public TableNotEmptyEventArgs(int seatIndex, string username, int chipCount, int buyIn) {
            SeatIndex = seatIndex;
            Username = username;
            ChipCount = chipCount;
            BuyIn = buyIn;
        }
    }
}