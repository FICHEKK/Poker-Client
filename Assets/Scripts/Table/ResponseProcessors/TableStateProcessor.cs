using System.Collections.Generic;
using Table.EventArguments;

namespace Table.ResponseProcessors
{
    public sealed partial class ServerConnectionHandler
    {
        private class TableStateProcessor : IServerResponseProcessor
        {
            public bool CanWait => false;
            
            private int dealerButtonIndex;
            private int smallBlind;
            private int maxPlayers;
            
            private readonly List<int> indexes      = new List<int>();
            private readonly List<string> usernames = new List<string>();
            private readonly List<int> stacks       = new List<int>();
            private readonly List<int> bets         = new List<int>();
            private readonly List<bool> folds       = new List<bool>();
            
            private readonly List<string> cards     = new List<string>();
            
            private int playerIndex;
            private int pot;
            
            public void ReadPayloadData()
            {
                dealerButtonIndex = Session.ReadInt();
                smallBlind = Session.ReadInt();
                maxPlayers = Session.ReadInt();
                
                ReceivePlayerList();
                ReceiveCommunityCardList();
                
                playerIndex = Session.ReadInt();
                pot = Session.ReadInt();
            }

            public void ProcessResponse(ServerConnectionHandler handler)
            {
                handler.TableInit?.Invoke(handler, new TableInitEventArgs(
                    indexes,
                    usernames,
                    stacks,
                    bets,
                    folds,
                    cards,
                    playerIndex,
                    pot,
                    dealerButtonIndex,
                    smallBlind,
                    maxPlayers)
                );
            }

            private void ReceivePlayerList()
            {
                var playerCount = Session.ReadInt();

                for (var i = 0; i < playerCount; i++)
                {
                    indexes  .Add(Session.ReadInt());
                    usernames.Add(Session.ReadLine());
                    stacks   .Add(Session.ReadInt());
                    bets     .Add(Session.ReadInt());
                    folds    .Add(Session.ReadBool());
                }
            }
            
            private void ReceiveCommunityCardList()
            {
                var cardCount = Session.ReadInt();

                for (var i = 0; i < cardCount; i++)
                {
                    cards.Add(Session.ReadLine());
                }
            }
        }
    }
}