namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class WaitForMillisecondsProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private int milliseconds;

            public void ReadPayloadData()
            {
                milliseconds = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.isWaitingMode = true;
                handler.waitingModeTimer.Interval = milliseconds;
                handler.waitingModeTimer.Start();
            }
        }
    }
}