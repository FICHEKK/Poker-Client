using System;
using System.Collections.Generic;
using System.Threading;
using Table.EventArguments;
using UnityEngine;

namespace Table
{
    /// <summary>
    /// Handles the server connection by receiving data from the network
    /// stream and raising events based on the data received.
    /// </summary>
    public sealed class ServerConnectionHandler : MonoBehaviour
    {
        public event EventHandler<TableInitEventArgs> TableInit;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;
        public event EventHandler<PlayerIndexEventArgs> PlayerIndex;
        public event EventHandler<PlayerCheckedEventArgs> PlayerChecked;
        public event EventHandler<PlayerCalledEventArgs> PlayerCalled;
        public event EventHandler<PlayerFoldedEventArgs> PlayerFolded;
        public event EventHandler<PlayerRaisedEventArgs> PlayerRaised;
        public event EventHandler<PlayerAllInEventArgs> PlayerAllIn;
        public event EventHandler<BlindsReceivedEventArgs> BlindsReceived;
        public event EventHandler<RequiredBetReceivedEventArgs> RequiredBetReceived;
        public event EventHandler<HandReceivedEventArgs> HandReceived;
        public event EventHandler<FlopReceivedEventArgs> FlopReceived;
        public event EventHandler<TurnReceivedEventArgs> TurnReceived;
        public event EventHandler<RiverReceivedEventArgs> RiverReceived;
        public event EventHandler<ShowdownEventArgs> Showdown;
        public event EventHandler<RoundFinishedEventArgs> RoundFinished;

        private readonly Dictionary<ServerResponse, Action> responseToAction = new Dictionary<ServerResponse, Action>();

        private void Awake()
        {
            responseToAction.Add(ServerResponse.Hand, 
                () => HandReceived?.Invoke(this, new HandReceivedEventArgs(Session.ReadLine(), Session.ReadLine())));
            
            responseToAction.Add(ServerResponse.Flop,
                () => FlopReceived?.Invoke(this, new FlopReceivedEventArgs(Session.ReadLine(), Session.ReadLine(), Session.ReadLine())));
            
            responseToAction.Add(ServerResponse.Turn,
                () => TurnReceived?.Invoke(this, new TurnReceivedEventArgs(Session.ReadLine())));
            
            responseToAction.Add(ServerResponse.River,
                () => RiverReceived?.Invoke(this, new RiverReceivedEventArgs(Session.ReadLine())));
            
            responseToAction.Add(ServerResponse.RoundFinished,
                () => RoundFinished?.Invoke(this, new RoundFinishedEventArgs()));
            
            responseToAction.Add(ServerResponse.PlayerChecked,
                () => PlayerChecked?.Invoke(this, new PlayerCheckedEventArgs(Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerCalled,
                () => PlayerCalled?.Invoke(this, new PlayerCalledEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerFolded,
                () => PlayerFolded?.Invoke(this, new PlayerFoldedEventArgs(Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerRaised,
                () => PlayerRaised?.Invoke(this, new PlayerRaisedEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerAllIn,
                () => PlayerAllIn?.Invoke(this, new PlayerAllInEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerIndex,
                () => PlayerIndex?.Invoke(this, new PlayerIndexEventArgs(Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.Blinds,
                () => BlindsReceived?.Invoke(this, new BlindsReceivedEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.RequiredBet,
                () => RequiredBetReceived?.Invoke(this, new RequiredBetReceivedEventArgs(Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerJoined,
                () => PlayerJoined?.Invoke(this, new PlayerJoinedEventArgs(Session.ReadInt(), Session.ReadLine(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerLeft,
                () => PlayerLeft?.Invoke(this, new PlayerLeftEventArgs(Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.Showdown,
                () =>
                {
                    int winnerCount = Session.ReadInt();
                    List<int> winnerIndexes = new List<int>();

                    for (int i = 0; i < winnerCount; i++)
                    {
                        winnerIndexes.Add(Session.ReadInt());
                    }

                    Showdown?.Invoke(this, new ShowdownEventArgs(winnerIndexes));
                });
        }

        private void Start()
        {
            new Thread(HandleServerConnection).Start();
        }

        private void HandleServerConnection()
        {
            InitializeTable();

            int flag = Session.Reader.Read();

            while (flag != -1)
            {
                if (responseToAction.TryGetValue((ServerResponse) flag, out var action))
                {
                    action();
                }

                flag = Session.Reader.Read();

                if ((ServerResponse) flag == ServerResponse.LeaveTableSuccess) break;
            }
        }

        private void InitializeTable()
        {
            int smallBlind = Session.ReadInt();
            int playerCount = Session.ReadInt();
            int maxPlayers = Session.ReadInt();

            List<TablePlayerData> players = new List<TablePlayerData>();

            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new TablePlayerData(Session.ReadInt(), Session.ReadLine(), Session.ReadInt()));
            }
            
            TableInit?.Invoke(this, new TableInitEventArgs(smallBlind, maxPlayers, players));
        }

        public class TablePlayerData
        {
            public int Index { get; }
            public string Username { get; }
            public int Stack { get; }

            public TablePlayerData(int index, string username, int stack)
            {
                Index = index;
                Username = username;
                Stack = stack;
            }
        }
    }
}
