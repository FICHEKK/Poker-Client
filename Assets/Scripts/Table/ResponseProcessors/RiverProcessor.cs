using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class RiverProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private string card;

            public void ReadPayloadData()
            {
                card = Session.ReadLine();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.RiverReceived?.Invoke(handler, new RiverReceivedEventArgs(card));
            }
        }
    }
}