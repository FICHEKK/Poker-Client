using System;

namespace Table.EventArguments
{
    public class RiverReceivedEventArgs : EventArgs
    {
        public string Card { get; }

        public RiverReceivedEventArgs(string card)
        {
            Card = card;
        }
    }
}