using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<PlayerCheckedEventArgs> PlayerChecked;
        
        private class PlayerCheckedProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int playerIndex;

            public void ReadPayloadData()
            {
                playerIndex = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerChecked?.Invoke(handler, new PlayerCheckedEventArgs(playerIndex));
            }
        }
    }
}