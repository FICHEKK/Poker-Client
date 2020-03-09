using System;
using System.Collections.Generic;

namespace Table.EventArguments
{
    public class TableInitEventArgs : EventArgs
    {
        public int SmallBlind { get; }
        public int MaxPlayers { get; }
        public List<ServerConnectionHandler.TablePlayerData> Players;

        public TableInitEventArgs(int smallBlind, int maxPlayers, List<ServerConnectionHandler.TablePlayerData> players)
        {
            SmallBlind = smallBlind;
            MaxPlayers = maxPlayers;
            Players = players;
        }
    }
}