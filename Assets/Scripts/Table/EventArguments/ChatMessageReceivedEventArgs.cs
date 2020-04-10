using System;

namespace Table.EventArguments
{
    public class ChatMessageReceivedEventArgs : EventArgs
    {
        public int PlayerIndex { get; }
        public string Message { get; }

        public ChatMessageReceivedEventArgs(int playerIndex, string message)
        {
            PlayerIndex = playerIndex;
            Message = message;
        }
    }
}