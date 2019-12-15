using System;

namespace Table.EventArguments {
    public class PlayerFoldedEventArgs : EventArgs {
        public int PlayerIndex { get; }
        
        public PlayerFoldedEventArgs(int playerIndex) {
            PlayerIndex = playerIndex;
        }
    }
}