using System;
using System.Threading;
using Table.EventArguments;

namespace Table {
    
    /// <summary>
    /// Handles the server connection by receiving data from the network
    /// stream and raising events based on the data received.
    /// </summary>
    public sealed class ServerConnectionHandler {
        public event EventHandler TableEmpty;
        public event EventHandler<TableNotEmptyEventArgs> TableNotEmpty;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;
        
        public event EventHandler<HandReceivedEventArgs> HandReceived;
        public event EventHandler<FlopReceivedEventArgs> FlopReceived;
        public event EventHandler<TurnReceivedEventArgs> TurnReceived;
        public event EventHandler<RiverReceivedEventArgs> RiverReceived;

        private bool handling;
        private int seatIndex;

        public void Handle() {
            if (handling) return;
            
            new Thread(HandleServerConnection).Start();
            handling = true;
        }

        private void HandleServerConnection() {
            InitializeTable();
            
            int responseCode = Session.Reader.Read();

            while (responseCode != -1) {
                ServerResponse response = (ServerResponse) responseCode;

                if (response == ServerResponse.Hand) {
                    OnHandReceived(new HandReceivedEventArgs(ReadLine(), ReadLine()));
                }
                else if (response == ServerResponse.Flop) {
                    OnFlopReceived(new FlopReceivedEventArgs(ReadLine(), ReadLine(), ReadLine()));
                }
                else if (response == ServerResponse.Turn) {
                    OnTurnReceived(new TurnReceivedEventArgs(ReadLine()));
                }
                else if (response == ServerResponse.River) {
                    OnRiverReceived(new RiverReceivedEventArgs(ReadLine()));
                }
                else if (response == ServerResponse.PlayerJoined) {
                    string username = ReadLine();
                    int chipCount = int.Parse(ReadLine());

                    if (username != Session.Username) {
                        OnPlayerJoined(new PlayerJoinedEventArgs(username, chipCount));
                    }
                }

                responseCode = Session.Reader.Read();
            }
        }

        private void InitializeTable() {
            ServerJoinTableResponse response = (ServerJoinTableResponse) Session.Reader.Read();

            if (response == ServerJoinTableResponse.TableEmpty) {
                OnTableEmpty();
            }
            else if(response == ServerJoinTableResponse.TableNotEmpty) {
                string username = ReadLine();
                int chipCount = int.Parse(ReadLine());
                OnTableNotEmpty(new TableNotEmptyEventArgs(username, chipCount));
            }
        }

        private string ReadLine() => Session.Reader.ReadLine();

        public void Check() => SendRequest(ClientRequest.Check);
        public void Call()  => SendRequest(ClientRequest.Call);
        public void Fold()  => SendRequest(ClientRequest.Fold);
        public void Raise() => SendRequest(ClientRequest.Raise);
        public void AllIn() => SendRequest(ClientRequest.AllIn);
        private static void SendRequest(ClientRequest request) => Session.Client.GetStream().WriteByte((byte) request);
        
        #region Events

        private void OnTableEmpty() => TableEmpty?.Invoke(this, EventArgs.Empty);
        private void OnTableNotEmpty(TableNotEmptyEventArgs args) => TableNotEmpty?.Invoke(this, args);
        private void OnPlayerJoined(PlayerJoinedEventArgs args) => PlayerJoined?.Invoke(this, args);
        private void OnPlayerLeft(PlayerLeftEventArgs args) => PlayerLeft?.Invoke(this, args);

        private void OnHandReceived(HandReceivedEventArgs args) => HandReceived?.Invoke(this, args);
        private void OnFlopReceived(FlopReceivedEventArgs args) => FlopReceived?.Invoke(this, args);
        private void OnTurnReceived(TurnReceivedEventArgs args) => TurnReceived?.Invoke(this, args);
        private void OnRiverReceived(RiverReceivedEventArgs args) => RiverReceived?.Invoke(this, args);

        #endregion
    }
}
