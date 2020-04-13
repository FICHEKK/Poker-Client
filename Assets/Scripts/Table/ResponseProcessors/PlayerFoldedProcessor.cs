using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<PlayerFoldedEventArgs> PlayerFolded;
        
        private class PlayerFoldedProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int playerIndex;

            public void ReadPayloadData()
            {
                playerIndex = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerFolded?.Invoke(handler, new PlayerFoldedEventArgs(playerIndex));
            }
        }
    }
}