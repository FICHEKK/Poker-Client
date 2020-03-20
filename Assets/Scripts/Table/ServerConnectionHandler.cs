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
        public event EventHandler<CardsRevealedEventArgs> CardsRevealed;

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
                () => BlindsReceived?.Invoke(this, new BlindsReceivedEventArgs(Session.ReadIntList(), Session.ReadInt(), Session.ReadInt(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.RequiredBet,
                () => RequiredBetReceived?.Invoke(this, new RequiredBetReceivedEventArgs(Session.ReadInt(), Session.ReadInt(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerJoined,
                () => PlayerJoined?.Invoke(this, new PlayerJoinedEventArgs(Session.ReadInt(), Session.ReadLine(), Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.PlayerLeft,
                () => PlayerLeft?.Invoke(this, new PlayerLeftEventArgs(Session.ReadInt())));
            
            responseToAction.Add(ServerResponse.Showdown,
                () =>
                {
                    int sidePotCount = Session.ReadInt();
                    var sidePots = new List<ShowdownEventArgs.Pot>(sidePotCount);
                    
                    for (int i = 0; i < sidePotCount; i++)
                        sidePots.Add(new ShowdownEventArgs.Pot(Session.ReadInt(), Session.ReadIntList()));

                    Showdown?.Invoke(this, new ShowdownEventArgs(sidePots));
                });
            
            responseToAction.Add(ServerResponse.CardsReveal,
                () =>
                {
                    int count = Session.ReadInt();

                    var indexes = new List<int>(count);
                    var firstCards = new List<string>(count);
                    var secondCards = new List<string>(count);
                    
                    for (int i = 0; i < count; i++)
                    {
                        indexes.Add(Session.ReadInt());
                        firstCards.Add(Session.ReadLine());
                        secondCards.Add(Session.ReadLine());
                    }
                    
                    CardsRevealed?.Invoke(this, new CardsRevealedEventArgs(indexes, firstCards, secondCards));
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

                if ((ServerResponse) flag == ServerResponse.WaitForMilliseconds)
                {
                    Thread.Sleep(Session.ReadInt());
                    flag = Session.Reader.Read();
                }
                else if ((ServerResponse) flag == ServerResponse.LeaveTableSuccess)
                {
                    break;
                }
            }
            
            MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
        }

        private void InitializeTable()
        {
            int dealerButtonIndex = Session.ReadInt();
            int smallBlind = Session.ReadInt();
            int maxPlayers = Session.ReadInt();
            
            var players = ReceivePlayerList();
            var cards = ReceiveCommunityCardList();
            
            int playerIndex = Session.ReadInt();
            int pot = Session.ReadInt();

            TableInit?.Invoke(this, new TableInitEventArgs(players, cards, playerIndex, pot, dealerButtonIndex, smallBlind, maxPlayers));
        }

        private static List<TablePlayerData> ReceivePlayerList()
        {
            int playerCount = Session.ReadInt();
            var players = new List<TablePlayerData>(playerCount);

            for (int i = 0; i < playerCount; i++)
            {
                int index = Session.ReadInt();
                string username = Session.ReadLine();
                int stack = Session.ReadInt();
                int bet = Session.ReadInt();
                bool folded = Session.ReadBool();
                
                players.Add(new TablePlayerData(index, username, stack, bet, folded));
            }

            return players;
        }

        private static List<string> ReceiveCommunityCardList()
        {
            int cardCount = Session.ReadInt();
            var cards = new List<string>(cardCount);

            for (int i = 0; i < cardCount; i++)
            {
                string card = Session.ReadLine();
                cards.Add(card);
            }

            return cards;
        }

        public class TablePlayerData
        {
            public int Index { get; }
            public string Username { get; }
            public int Stack { get; }
            public int Bet { get; }
            public bool Folded { get; }

            public TablePlayerData(int index, string username, int stack, int bet, bool folded)
            {
                Index = index;
                Username = username;
                Stack = stack;
                Bet = bet;
                Folded = folded;
            }
        }
    }
}
