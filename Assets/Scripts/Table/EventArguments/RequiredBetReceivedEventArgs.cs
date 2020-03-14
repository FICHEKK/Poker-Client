using System;

namespace Table.EventArguments
{
    public class RequiredBetReceivedEventArgs : EventArgs
    {
        public int RequiredCall { get; }
        public int MinRaise { get; }
        public int MaxRaise { get; }

        public RequiredBetReceivedEventArgs(int requiredCall, int minRaise, int maxRaise)
        {
            RequiredCall = requiredCall;
            MinRaise = minRaise;
            MaxRaise = maxRaise;
        }
    }
}