﻿using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class PlayerIndexProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int index;

            public void ReadPayloadData()
            {
                index = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerIndex?.Invoke(handler, new PlayerIndexEventArgs(index));
            }
        }
    }
}