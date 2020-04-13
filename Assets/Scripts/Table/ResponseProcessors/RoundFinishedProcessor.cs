using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<RoundFinishedEventArgs> RoundFinished;
        
        private class RoundFinishedProcessor : IServerResponseProcessor
        {
            public void ReadPayloadData()
            {
                // no data to read
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.RoundFinished?.Invoke(handler, new RoundFinishedEventArgs());
            }
        }
    }
}