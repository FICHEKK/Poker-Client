using System;
using System.Collections.Generic;
using static Table.ResponseProcessors.ServerConnectionHandler;

namespace Table.EventArguments
{
    public class TableInitEventArgs : EventArgs
    {
        public readonly List<TablePlayerData> Players;
        public readonly List<string> CommunityCards;
        public int PlayerIndex { get; }
        public int Pot { get; }
        public int DealerButtonIndex { get; }
        public int SmallBlind { get; }
        public int MaxPlayers { get; }

        public TableInitEventArgs(List<TablePlayerData> players, List<string> communityCards,
            int playerIndex, int pot, int dealerButtonIndex, int smallBlind, int maxPlayers)
        {
            Players = players;
            CommunityCards = communityCards;
            PlayerIndex = playerIndex;
            Pot = pot;
            DealerButtonIndex = dealerButtonIndex;
            SmallBlind = smallBlind;
            MaxPlayers = maxPlayers;
        }
    }
}