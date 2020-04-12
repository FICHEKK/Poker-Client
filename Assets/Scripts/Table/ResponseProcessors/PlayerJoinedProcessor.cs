﻿using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class PlayerJoinedProcessor : IServerResponseProcessor
        {
            public bool CanWait => false;
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