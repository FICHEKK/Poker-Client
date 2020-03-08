using System;

namespace Table.EventArguments
{
    public class TableNotEmptyEventArgs : EventArgs
    {
        public int SeatIndex { get; }
        public int SmallBlind { get; }
        public int BuyIn { get; }

        public string OpponentUsername { get; }
        public int OpponentStack { get; }


        public TableNotEmptyEventArgs(int seatIndex, int smallBlind, int buyIn, string opponentUsername, int opponentStack)
        {
            SeatIndex = seatIndex;
            SmallBlind = smallBlind;
            BuyIn = buyIn;
            OpponentUsername = opponentUsername;
            OpponentStack = opponentStack;
        }
    }
}