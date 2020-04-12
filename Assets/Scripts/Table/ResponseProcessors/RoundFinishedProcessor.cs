using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class RoundFinishedProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;

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