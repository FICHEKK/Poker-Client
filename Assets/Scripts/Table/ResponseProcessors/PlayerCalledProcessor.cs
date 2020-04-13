using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<PlayerCalledEventArgs> PlayerCalled;
        
        private class PlayerCalledProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int playerIndex;
            private int callAmount;

            public void ReadPayloadData()
            {
                playerIndex = Session.ReadInt();
                callAmount = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerCalled?.Invoke(handler, new PlayerCalledEventArgs(playerIndex, callAmount));
            }
        }
    }
}