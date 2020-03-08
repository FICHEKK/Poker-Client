using System;

namespace Table.EventArguments
{
    public class PlayerAllInEventArgs : EventArgs
    {
        public int PlayerIndex { get; }
        public int AllInAmount { get; }

        public PlayerAllInEventArgs(int playerIndex, int allInAmount)
        {
            PlayerIndex = playerIndex;
            AllInAmount = allInAmount;
        }
    }
}