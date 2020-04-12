using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class PlayerRaisedProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int playerIndex;
            private int raisedToAmount;

            public void ReadPayloadData()
            {
                playerIndex = Session.ReadInt();
                raisedToAmount = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerRaised?.Invoke(handler, new PlayerRaisedEventArgs(playerIndex, raisedToAmount));
            }
        }
    }
}