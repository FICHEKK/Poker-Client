using System;

namespace Table.EventArguments
{
    public class PlayerLeftEventArgs : EventArgs
    {
        public int Index { get; }

        public PlayerLeftEventArgs(int index)
        {
            Index = index;
        }
    }
}