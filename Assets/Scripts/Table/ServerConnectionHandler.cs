using System;
using System.Threading;
using Table.EventArguments;

namespace Table {
    
    /// <summary>
    /// Handles the server connection by receiving data from the network
    /// stream and raising events based on the data received.
    /// </summary>
    public sealed class ServerConnectionHandler {
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

        private bool handling;

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
                else if (response == ServerResponse.PlayerChecked) {
                    int playerIndex = int.Parse(ReadLine());
                    OnPlayerChecked(new PlayerCheckedEventArgs(playerIndex));
                }
                else if (response == ServerResponse.PlayerCalled) {
                    int playerIndex = int.Parse(ReadLine());
                    int callAmount = int.Parse(ReadLine());
                    OnPlayerCalled(new PlayerCalledEventArgs(playerIndex, callAmount));
                }
                else if (response == ServerResponse.PlayerFolded) {
                    int playerIndex = int.Parse(ReadLine());
                    OnPlayerFolded(new PlayerFoldedEventArgs(playerIndex));
                }
                else if (response == ServerResponse.PlayerRaised) {
                    int playerIndex = int.Parse(ReadLine());
                    int raiseAmount = int.Parse(ReadLine());
                    OnPlayerRaised(new PlayerRaisedEventArgs(playerIndex, raiseAmount));
                }
                else if (response == ServerResponse.PlayerAllIn) {
                    int playerIndex = int.Parse(ReadLine());
                    int allInAmount = int.Parse(ReadLine());
                    OnPlayerAllIn(new PlayerAllInEventArgs(playerIndex, allInAmount));
                }
                else if (response == ServerResponse.PlayerIndex) {
                    int playerIndex = int.Parse(ReadLine());
                    OnPlayerIndex(new PlayerIndexEventArgs(playerIndex));
                }
                else if (response == ServerResponse.Blinds) {
                    int smallBlindIndex = int.Parse(ReadLine());
                    int bigBlindIndex = int.Parse(ReadLine());
                    OnBlindsReceived(new BlindsReceivedEventArgs(smallBlindIndex, bigBlindIndex));
                }
                else if (response == ServerResponse.RequiredBet) {
                    int requiredBet = int.Parse(ReadLine());
                    OnRequiredBetReceived(new RequiredBetReceivedEventArgs(requiredBet));
                }
                else if (response == ServerResponse.Showdown) {
                    OnShowdown(new ShowdownEventArgs());
                }
                else if (response == ServerResponse.RoundFinished) {
                    OnRoundFinished(new RoundFinishedEventArgs());
                }

                responseCode = Session.Reader.Read();
            }
        }

        private void InitializeTable() {
            int seatIndex = int.Parse(ReadLine());
            int smallBlind = int.Parse(ReadLine());
            int buyIn = int.Parse(ReadLine());
            ServerJoinTableResponse response = (ServerJoinTableResponse) Session.Reader.Read();

            if (response == ServerJoinTableResponse.TableEmpty) {
                OnTableEmpty(new TableEmptyEventArgs(seatIndex, smallBlind, buyIn));
            }
            else if(response == ServerJoinTableResponse.TableNotEmpty) {
                string opponentUsername = ReadLine();
                int opponentStack = int.Parse(ReadLine());
                OnTableNotEmpty(new TableNotEmptyEventArgs(seatIndex, smallBlind, buyIn, opponentUsername, opponentStack));
            }
        }

        private string ReadLine() => Session.Reader.ReadLine();
        
        #region Events

        private void OnTableEmpty(TableEmptyEventArgs args) => TableEmpty?.Invoke(this, args);
        private void OnTableNotEmpty(TableNotEmptyEventArgs args) => TableNotEmpty?.Invoke(this, args);
        
        private void OnPlayerJoined(PlayerJoinedEventArgs args) => PlayerJoined?.Invoke(this, args);
        private void OnPlayerLeft(PlayerLeftEventArgs args) => PlayerLeft?.Invoke(this, args);
        private void OnPlayerIndex(PlayerIndexEventArgs args) => PlayerIndex?.Invoke(this, args);

        private void OnPlayerChecked(PlayerCheckedEventArgs args) => PlayerChecked?.Invoke(this, args);
        private void OnPlayerCalled(PlayerCalledEventArgs args) => PlayerCalled?.Invoke(this, args);
        private void OnPlayerFolded(PlayerFoldedEventArgs args) => PlayerFolded?.Invoke(this, args);
        private void OnPlayerRaised(PlayerRaisedEventArgs args) => PlayerRaised?.Invoke(this, args);
        private void OnPlayerAllIn(PlayerAllInEventArgs args) => PlayerAllIn?.Invoke(this, args);
        
        private void OnBlindsReceived(BlindsReceivedEventArgs args) => BlindsReceived?.Invoke(this, args);
        private void OnRequiredBetReceived(RequiredBetReceivedEventArgs args) => RequiredBetReceived?.Invoke(this, args);
        private void OnHandReceived(HandReceivedEventArgs args) => HandReceived?.Invoke(this, args);
        private void OnFlopReceived(FlopReceivedEventArgs args) => FlopReceived?.Invoke(this, args);
        private void OnTurnReceived(TurnReceivedEventArgs args) => TurnReceived?.Invoke(this, args);
        private void OnRiverReceived(RiverReceivedEventArgs args) => RiverReceived?.Invoke(this, args);
        private void OnShowdown(ShowdownEventArgs args) => Showdown?.Invoke(this, args);
        private void OnRoundFinished(RoundFinishedEventArgs args) => RoundFinished?.Invoke(this, args);

        #endregion
    }
}
