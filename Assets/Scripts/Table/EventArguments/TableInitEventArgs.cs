using System;
using System.Collections.Generic;
using static Table.ResponseProcessors.ServerConnectionHandler;

namespace Table.EventArguments
{
    public class TableInitEventArgs : EventArgs
    {
        public readonly List<string> CommunityCards;
        public int PlayerIndex { get; }
        public int Pot { get; }
        public int DealerButtonIndex { get; }
        public int SmallBlind { get; }
        public int MaxPlayers { get; }
        
        public List<int> Indexes { get; }
        public List<string> Usernames { get; }
        public List<int> Stacks { get; }
        public List<int> Bets { get; }
        public List<bool> Folds { get; }

        public TableInitEventArgs(
            List<int> indexes,
            List<string> usernames,
            List<int> stacks,
            List<int> bets,
            List<bool> folds,
            List<string> communityCards,
            int playerIndex,
            int pot,
            int dealerButtonIndex,
            int smallBlind,
            int maxPlayers)
        {
            Indexes = indexes;
            Usernames = usernames;
            Stacks = stacks;
            Bets = bets;
            Folds = folds;
            CommunityCards = communityCards;
            PlayerIndex = playerIndex;
            Pot = pot;
            DealerButtonIndex = dealerButtonIndex;
            SmallBlind = smallBlind;
            MaxPlayers = maxPlayers;
        }
    }
}