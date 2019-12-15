using System;

namespace Table.EventArguments {
    public class PlayerCalledEventArgs : EventArgs {
        public int PlayerIndex { get; }
        public int CallAmount { get; }
        
        public PlayerCalledEventArgs(int playerIndex, int callAmount) {
            PlayerIndex = playerIndex;
            CallAmount = callAmount;
        }
    }
}