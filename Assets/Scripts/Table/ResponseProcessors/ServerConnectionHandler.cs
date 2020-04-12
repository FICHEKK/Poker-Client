using System;
using System.Collections.Generic;
using System.Threading;
using Table.EventArguments;
using UnityEngine;
using Timer = System.Timers.Timer;

namespace Table.ResponseProcessors
{
    /// <summary>
    /// Handles the server connection by receiving data from the network
    /// stream and raising events based on the data received.
    /// </summary>
    public sealed partial class ServerConnectionHandler : MonoBehaviour
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
        public event EventHandler<ChatMessageReceivedEventArgs> ChatMessageReceived;

        private readonly Dictionary<ServerResponse, IServerResponseProcessor> responseToProcessor = new Dictionary<ServerResponse, IServerResponseProcessor>();
        
        private bool isWaitingMode;
        private readonly Queue<Action<ServerConnectionHandler>> waitingModeQueue = new Queue<Action<ServerConnectionHandler>>();
        private readonly Timer waitingModeTimer = new Timer();

        private void Awake()
        {
            waitingModeTimer.Elapsed += (sender, e) =>
            {
                while (waitingModeQueue.Count > 0)
                {
                    waitingModeQueue.Dequeue()(this);
                }
                
                isWaitingMode = false;
            };
                
            responseToProcessor.Add(ServerResponse.Hand, new HandProcessor());
            responseToProcessor.Add(ServerResponse.Flop, new FlopProcessor());
            responseToProcessor.Add(ServerResponse.Turn, new TurnProcessor());
            responseToProcessor.Add(ServerResponse.River, new RiverProcessor());
            responseToProcessor.Add(ServerResponse.RoundFinished, new RoundFinishedProcessor());
            responseToProcessor.Add(ServerResponse.PlayerChecked, new PlayerCheckedProcessor());
            responseToProcessor.Add(ServerResponse.PlayerCalled, new PlayerCalledProcessor());
            responseToProcessor.Add(ServerResponse.PlayerFolded, new PlayerFoldedProcessor());
            responseToProcessor.Add(ServerResponse.PlayerRaised, new PlayerRaisedProcessor());
            responseToProcessor.Add(ServerResponse.PlayerAllIn, new PlayerAllInProcessor());
            responseToProcessor.Add(ServerResponse.PlayerIndex, new PlayerIndexProcessor());
            responseToProcessor.Add(ServerResponse.Blinds, new BlindsProcessor());
            responseToProcessor.Add(ServerResponse.RequiredBet, new RequiredBetProcessor());
            responseToProcessor.Add(ServerResponse.PlayerJoined, new PlayerJoinedProcessor());
            responseToProcessor.Add(ServerResponse.PlayerLeft, new PlayerLeftProcessor());
            responseToProcessor.Add(ServerResponse.Showdown, new ShowdownProcessor());
            responseToProcessor.Add(ServerResponse.CardsReveal, new CardsRevealProcessor());
            responseToProcessor.Add(ServerResponse.ChatMessage, new ChatMessageProcessor());
            responseToProcessor.Add(ServerResponse.WaitForMilliseconds, new WaitForMillisecondsProcessor());
        }

        private void Start()
        {
            new Thread(HandleServerConnection).Start();
        }

        private void HandleServerConnection()
        {
            InitializeTable();

            int flag = Session.Reader.Read();

            while (flag != -1 && (ServerResponse) flag != ServerResponse.LeaveTableSuccess)
            {
                if (responseToProcessor.TryGetValue((ServerResponse) flag, out var processor))
                {
                    processor.ReadPayloadData();
                    
                    if (isWaitingMode && processor.CanWait)
                    {
                        waitingModeQueue.Enqueue(processor.ProcessResponse);
                    }
                    else
                    {
                        processor.ProcessResponse(this);
                    }
                }

                flag = Session.Reader.Read();
            }
            
            waitingModeTimer.Stop();
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
