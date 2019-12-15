using System;

namespace Table.EventArguments {
    public class PlayerRaisedEventArgs : EventArgs {
        public int PlayerIndex { get; }
        public int RaiseAmount { get; }

        public PlayerRaisedEventArgs(int playerIndex, int raiseAmount) {
            PlayerIndex = playerIndex;
            RaiseAmount = raiseAmount;
        }
    }
}