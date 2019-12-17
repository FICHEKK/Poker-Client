using System;

namespace Table.EventArguments {
    public class RequiredBetReceivedEventArgs : EventArgs {
        public int RequiredBet { get; }

        public RequiredBetReceivedEventArgs(int requiredBet) {
            RequiredBet = requiredBet;
        }
    }
}