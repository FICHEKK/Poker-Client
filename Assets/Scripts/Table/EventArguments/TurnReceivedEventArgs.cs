using System;

namespace Table.EventArguments
{
    public class TurnReceivedEventArgs : EventArgs
    {
        public string Card { get; }

        public TurnReceivedEventArgs(string card)
        {
            Card = card;
        }
    }
}