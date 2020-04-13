using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;
        
        private class PlayerLeftProcessor : IServerResponseProcessor
        {
            private int index;

            public void ReadPayloadData()
            {
                index = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerLeft?.Invoke(handler, new PlayerLeftEventArgs(index));
            }
        }
    }
}