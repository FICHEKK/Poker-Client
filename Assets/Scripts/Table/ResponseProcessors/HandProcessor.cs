using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class HandProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private string card1;
            private string card2;

            public void ReadPayloadData()
            {
                card1 = Session.ReadLine();
                card2 = Session.ReadLine();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.HandReceived?.Invoke(handler, new HandReceivedEventArgs(card1, card2));
            }
        }
    }
}