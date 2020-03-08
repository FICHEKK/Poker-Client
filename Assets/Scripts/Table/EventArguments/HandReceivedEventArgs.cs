using System;

namespace Table.EventArguments
{
    public class HandReceivedEventArgs : EventArgs
    {
        public string Card1 { get; }
        public string Card2 { get; }

        public HandReceivedEventArgs(string card1, string card2)
        {
            Card1 = card1;
            Card2 = card2;
        }
    }
}