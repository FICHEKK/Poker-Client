using System;
using System.Collections.Generic;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<BlindsReceivedEventArgs> BlindsReceived;
        
        private class BlindsProcessor : IServerResponseProcessor
        {
            private List<int> justJoinedPlayerIndexes;
            private int dealerButtonIndex;
            private int smallBlindIndex;
            private int bigBlindIndex;

            public void ReadPayloadData()
            {
                justJoinedPlayerIndexes = Session.ReadIntList();
                dealerButtonIndex = Session.ReadInt();
                smallBlindIndex = Session.ReadInt();
                bigBlindIndex = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.BlindsReceived?.Invoke(handler, new BlindsReceivedEventArgs(
                    justJoinedPlayerIndexes,
                    dealerButtonIndex,
                    smallBlindIndex,
                    bigBlindIndex)
                );
            }
        }
    }
}