using System;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<FlopReceivedEventArgs> FlopReceived;
        
        private class FlopProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private string card1;
            private string card2;
            private string card3;

            public void ReadPayloadData()
            {
                card1 = Session.ReadLine();
                card2 = Session.ReadLine();
                card3 = Session.ReadLine();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.FlopReceived?.Invoke(handler, new FlopReceivedEventArgs(card1, card2, card3));
            }
        }
    }
}