using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Table.EventArguments;
using UnityEngine;

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
        
        private bool shouldProcessResponses = true;
        private readonly BlockingCollection<Action<ServerConnectionHandler>> actionBlockingQueue = new BlockingCollection<Action<ServerConnectionHandler>>();

        private void Awake()
        {
            responseToProcessor.Add(ServerResponse.TableState, new TableStateProcessor());
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
            new Thread(HandleWaitableResponses).Start();
        }

        private void HandleServerConnection()
        {
            int flag = Session.Reader.Read();

            while (flag != -1 && (ServerResponse) flag != ServerResponse.LeaveTableSuccess)
            {
                if (responseToProcessor.TryGetValue((ServerResponse) flag, out var processor))
                {
                    processor.ReadPayloadData();
                    
                    if (processor.CanWait)
                    {
                        actionBlockingQueue.Add(processor.ProcessResponse);
                    }
                    else
                    {
                        processor.ProcessResponse(this);
                    }
                }

                flag = Session.Reader.Read();
            }

            shouldProcessResponses = false;
            MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
        }

        private void HandleWaitableResponses()
        {
            while (shouldProcessResponses)
            {
                actionBlockingQueue.Take()(this);
            }
        }
    }
}
