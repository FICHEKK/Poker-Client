using System;

namespace Table.EventArguments
{
    public class PlayerRaisedEventArgs : EventArgs
    {
        public int PlayerIndex { get; }
        public int RaisedToAmount { get; }

        public PlayerRaisedEventArgs(int playerIndex, int raisedToAmount)
        {
            PlayerIndex = playerIndex;
            RaisedToAmount = raisedToAmount;
        }
    }
}