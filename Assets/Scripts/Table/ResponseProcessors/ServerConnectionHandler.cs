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
        /// <summary>Maps server response to the appropriate processor for resolving the server response.</summary>
        private readonly Dictionary<ServerResponse, IServerResponseProcessor> responseToProcessor
            = new Dictionary<ServerResponse, IServerResponseProcessor>();

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
        }

        private void Start()
        {
            new Thread(HandleServerResponses).Start();
        }

        private void HandleServerResponses()
        {
            int flag = Session.Reader.Read();

            while (flag != -1 && (ServerResponse) flag != ServerResponse.LeaveTableSuccess)
            {
                if (responseToProcessor.TryGetValue((ServerResponse) flag, out var processor))
                {
                    processor.ReadPayloadData();
                    processor.ProcessResponse(this);
                }

                flag = Session.Reader.Read();
            }
            
            MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
        }
    }
}
