using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<ChatMessageReceivedEventArgs> ChatMessageReceived;
        
        private class ChatMessageProcessor : IServerResponseProcessor
        {
            private int playerIndex;
            private string message;

            public void ReadPayloadData()
            {
                playerIndex = Session.ReadInt();
                message = Session.ReadLine();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.ChatMessageReceived?.Invoke(handler, new ChatMessageReceivedEventArgs(playerIndex, message));
            }
        }
    }
}