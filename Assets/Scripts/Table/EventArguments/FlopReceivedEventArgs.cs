using System;

namespace Table.EventArguments {
    public class FlopReceivedEventArgs : EventArgs {
        public string Card1 { get; }
        public string Card2 { get; }
        public string Card3 { get; }

        public FlopReceivedEventArgs(string card1, string card2, string card3) {
            Card1 = card1;
            Card2 = card2;
            Card3 = card3;
        }
    }
}