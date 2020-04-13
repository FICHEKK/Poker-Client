using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Table.ResponseProcessors
{
    /// <summary>
    /// Handles the server connection by receiving data from the network
    /// stream and raising events based on the data received.
    /// </summary>
    public sealed partial class ServerConnectionHandler : MonoBehaviour
    {
        /// <summary>Maps server response to factory method for creating the appropriate response processor.</summary>
        private readonly Dictionary<ServerResponse, Func<IServerResponseProcessor>> responseToProcessorFactory
            = new Dictionary<ServerResponse, Func<IServerResponseProcessor>>();

        /// <summary>A blocking queue used to store unprocessed server responses.</summary>
        private readonly BlockingCollection<Action<ServerConnectionHandler>> actionBlockingQueue 
            = new BlockingCollection<Action<ServerConnectionHandler>>();

        /// <summary>A dummy action that is used to indicate the end of the consuming.</summary>
        private static readonly Action<ServerConnectionHandler> PoisonPill = handler => {};

        private void Awake()
        {
            responseToProcessorFactory.Add(ServerResponse.TableState, () => new TableStateProcessor());
            responseToProcessorFactory.Add(ServerResponse.Hand, () => new HandProcessor());
            responseToProcessorFactory.Add(ServerResponse.Flop, () => new FlopProcessor());
            responseToProcessorFactory.Add(ServerResponse.Turn, () => new TurnProcessor());
            responseToProcessorFactory.Add(ServerResponse.River, () => new RiverProcessor());
            responseToProcessorFactory.Add(ServerResponse.RoundFinished, () => new RoundFinishedProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerChecked, () => new PlayerCheckedProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerCalled, () => new PlayerCalledProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerFolded, () => new PlayerFoldedProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerRaised, () => new PlayerRaisedProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerAllIn, () => new PlayerAllInProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerIndex, () => new PlayerIndexProcessor());
            responseToProcessorFactory.Add(ServerResponse.Blinds, () => new BlindsProcessor());
            responseToProcessorFactory.Add(ServerResponse.RequiredBet, () => new RequiredBetProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerJoined, () => new PlayerJoinedProcessor());
            responseToProcessorFactory.Add(ServerResponse.PlayerLeft, () => new PlayerLeftProcessor());
            responseToProcessorFactory.Add(ServerResponse.Showdown, () => new ShowdownProcessor());
            responseToProcessorFactory.Add(ServerResponse.CardsReveal, () => new CardsRevealProcessor());
            responseToProcessorFactory.Add(ServerResponse.ChatMessage, () => new ChatMessageProcessor());
            responseToProcessorFactory.Add(ServerResponse.WaitForMilliseconds, () => new WaitForMillisecondsProcessor());
        }

        private void Start()
        {
            new Thread(ProduceServerResponses).Start();
            new Thread(ConsumeServerResponses).Start();
        }

        private void ProduceServerResponses()
        {
            int flag = Session.Reader.Read();

            while (flag != -1 && (ServerResponse) flag != ServerResponse.LeaveTableSuccess)
            {
                if (responseToProcessorFactory.TryGetValue((ServerResponse) flag, out var processorFactory))
                {
                    var processor = processorFactory();
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
            
            actionBlockingQueue.Add(PoisonPill);
            MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
        }

        private void ConsumeServerResponses()
        {
            while (true)
            {
                var action = actionBlockingQueue.Take();
                if (action == PoisonPill) return;
                action(this);
            }
        }
    }
}
