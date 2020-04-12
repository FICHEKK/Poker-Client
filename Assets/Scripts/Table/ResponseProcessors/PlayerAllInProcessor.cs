using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class PlayerAllInProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int playerIndex;
            private int allInAmount;

            public void ReadPayloadData()
            {
                playerIndex = Session.ReadInt();
                allInAmount = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.PlayerAllIn?.Invoke(handler, new PlayerAllInEventArgs(playerIndex, allInAmount));
            }
        }
    }
}