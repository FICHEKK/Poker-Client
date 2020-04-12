using System.Collections.Generic;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class CardsRevealProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private List<int> indexes;
            private List<string> firstCards;
            private List<string> secondCards;

            public void ReadPayloadData()
            {
                var count = Session.ReadInt();

                indexes = new List<int>(count);
                firstCards = new List<string>(count);
                secondCards = new List<string>(count);

                for (var i = 0; i < count; i++)
                {
                    indexes.Add(Session.ReadInt());
                    firstCards.Add(Session.ReadLine());
                    secondCards.Add(Session.ReadLine());
                }
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.CardsRevealed?.Invoke(handler, new CardsRevealedEventArgs(indexes, firstCards, secondCards));
            }
        }
    }
}