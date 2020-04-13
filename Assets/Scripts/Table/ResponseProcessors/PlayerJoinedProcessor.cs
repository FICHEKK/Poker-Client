using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        
        private class PlayerJoinedProcessor : IServerResponseProcessor
        {
            private int index;
            private string username;
            private int stack;

            public void ReadPayloadData()
            {
                index = Session.ReadInt();
                username = Session.ReadLine();
                stack = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerJoined?.Invoke(handler, new PlayerJoinedEventArgs(index, username, stack));
            }
        }
    }
}