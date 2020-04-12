﻿using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class RequiredBetProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int requiredCall;
            private int minRaise;
            private int maxRaise;

            public void ReadPayloadData()
            {
                requiredCall = Session.ReadInt();
                minRaise = Session.ReadInt();
                maxRaise = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.RequiredBetReceived?.Invoke(handler, new RequiredBetReceivedEventArgs(requiredCall, minRaise, maxRaise));
            }
        }
    }
}