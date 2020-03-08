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
        public event EventHandler<TableEmptyEventArgs> TableEmpty;
        public event EventHandler<TableNotEmptyEventArgs> TableNotEmpty;
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

        private static readonly Dictionary<ServerResponse, Action> ResponseToAction =
            new Dictionary<ServerResponse, Action>();

        private void Awake()
        {
            if (ResponseToAction.Count != 0) return;
            
            ResponseToAction.Add(ServerResponse.Hand, 
                () => HandReceived?.Invoke(this, new HandReceivedEventArgs(Session.ReadLine(), Session.ReadLine())));
            
            ResponseToAction.Add(ServerResponse.Flop,
                () => FlopReceived?.Invoke(this, new FlopReceivedEventArgs(Session.ReadLine(), Session.ReadLine(), Session.ReadLine())));
            
            ResponseToAction.Add(ServerResponse.Turn,
                () => TurnReceived?.Invoke(this, new TurnReceivedEventArgs(Session.ReadLine())));
            
            ResponseToAction.Add(ServerResponse.River,
                () => RiverReceived?.Invoke(this, new RiverReceivedEventArgs(Session.ReadLine())));
            
            ResponseToAction.Add(ServerResponse.RoundFinished,
                () => RoundFinished?.Invoke(this, new RoundFinishedEventArgs()));
            
            ResponseToAction.Add(ServerResponse.PlayerChecked,
                () => PlayerChecked?.Invoke(this, new PlayerCheckedEventArgs(Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.PlayerCalled,
                () => PlayerCalled?.Invoke(this, new PlayerCalledEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.PlayerFolded,
                () => PlayerFolded?.Invoke(this, new PlayerFoldedEventArgs(Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.PlayerRaised,
                () => PlayerRaised?.Invoke(this, new PlayerRaisedEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.PlayerAllIn,
                () => PlayerAllIn?.Invoke(this, new PlayerAllInEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.PlayerIndex,
                () => PlayerIndex?.Invoke(this, new PlayerIndexEventArgs(Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.Blinds,
                () => BlindsReceived?.Invoke(this, new BlindsReceivedEventArgs(Session.ReadInt(), Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.RequiredBet,
                () => RequiredBetReceived?.Invoke(this, new RequiredBetReceivedEventArgs(Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.PlayerJoined,
                () => PlayerJoined?.Invoke(this, new PlayerJoinedEventArgs(Session.ReadLine(), Session.ReadInt())));
            
            ResponseToAction.Add(ServerResponse.Showdown,
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
                if (ResponseToAction.TryGetValue((ServerResponse) flag, out var action))
                {
                    action();
                }

                flag = Session.Reader.Read();
            }
        }

        private void InitializeTable()
        {
            int seatIndex = Session.ReadInt();
            int smallBlind = Session.ReadInt();
            int buyIn = Session.ReadInt();
            ServerJoinTableResponse response = (ServerJoinTableResponse) Session.Reader.Read();

            if (response == ServerJoinTableResponse.TableEmpty)
            {
                TableEmpty?.Invoke(this, new TableEmptyEventArgs(seatIndex, smallBlind, buyIn));
            }
            else if (response == ServerJoinTableResponse.TableNotEmpty)
            {
                string opponentUsername = Session.ReadLine();
                int opponentStack = Session.ReadInt();
                TableNotEmpty?.Invoke(this, new TableNotEmptyEventArgs(seatIndex, smallBlind, buyIn, opponentUsername, opponentStack));
            }
        }
    }
}
