using System;
using System.Collections.Generic;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        public event EventHandler<ShowdownEventArgs> Showdown;
        
        private class ShowdownProcessor : IServerResponseProcessor
        {
            public bool CanWait => true;
            private List<ShowdownEventArgs.Pot> sidePots;

            public void ReadPayloadData()
            {
                var sidePotCount = Session.ReadInt();
                sidePots = new List<ShowdownEventArgs.Pot>(sidePotCount);

                for (var i = 0; i < sidePotCount; i++)
                {
                    var potValue = Session.ReadInt();
                    var winnerIndexes = Session.ReadIntList();
                    var bestHand = Session.ReadLine();
                    sidePots.Add(new ShowdownEventArgs.Pot(potValue, winnerIndexes, bestHand));
                }
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.Showdown?.Invoke(handler, new ShowdownEventArgs(sidePots));
            }
        }
    }
}