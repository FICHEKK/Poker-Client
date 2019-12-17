using System;

namespace Table.EventArguments {
    public class BlindsReceivedEventArgs : EventArgs {
        public int SmallBlindIndex { get; }
        public int BigBlindIndex { get; }

        public BlindsReceivedEventArgs(int smallBlindIndex, int bigBlindIndex) {
            SmallBlindIndex = smallBlindIndex;
            BigBlindIndex = bigBlindIndex;
        }
    }
}