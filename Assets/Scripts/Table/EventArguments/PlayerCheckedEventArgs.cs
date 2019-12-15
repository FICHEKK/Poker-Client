using System;

namespace Table.EventArguments {
    public class PlayerCheckedEventArgs : EventArgs {
        public int PlayerIndex { get; }

        public PlayerCheckedEventArgs(int playerIndex) {
            PlayerIndex = playerIndex;
        }
    }
}