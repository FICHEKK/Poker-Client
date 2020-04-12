using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class PlayerLeftProcessor : IServerResponseProcessor
        {
            public bool CanWait => false;
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