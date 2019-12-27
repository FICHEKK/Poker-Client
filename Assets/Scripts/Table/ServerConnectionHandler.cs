using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Trace.WriteLine(responseCode + " -> " + response);

                if (response == ServerResponse.Hand) {
                    HandReceived?.Invoke(this, new HandReceivedEventArgs(ReadLine(), ReadLine()));
                }
                else if (response == ServerResponse.Flop) {
                    FlopReceived?.Invoke(this, new FlopReceivedEventArgs(ReadLine(), ReadLine(), ReadLine()));
                }
                else if (response == ServerResponse.Turn) {
                    TurnReceived?.Invoke(this, new TurnReceivedEventArgs(ReadLine()));
                }
                else if (response == ServerResponse.River) {
                    RiverReceived?.Invoke(this, new RiverReceivedEventArgs(ReadLine()));
                }
                else if (response == ServerResponse.PlayerJoined) {
                    string username = ReadLine();
                    int chipCount = ReadInt();

                    if (username != Session.Username) {
                        PlayerJoined?.Invoke(this, new PlayerJoinedEventArgs(username, chipCount));
                    }
                }
                else if (response == ServerResponse.PlayerChecked) {
                    PlayerChecked?.Invoke(this, new PlayerCheckedEventArgs(ReadInt()));
                }
                else if (response == ServerResponse.PlayerCalled) {
                    PlayerCalled?.Invoke(this, new PlayerCalledEventArgs(ReadInt(), ReadInt()));
                }
                else if (response == ServerResponse.PlayerFolded) {
                    PlayerFolded?.Invoke(this, new PlayerFoldedEventArgs(ReadInt()));
                }
                else if (response == ServerResponse.PlayerRaised) {
                    PlayerRaised?.Invoke(this, new PlayerRaisedEventArgs(ReadInt(), ReadInt()));
                }
                else if (response == ServerResponse.PlayerAllIn) {
                    PlayerAllIn?.Invoke(this, new PlayerAllInEventArgs(ReadInt(), ReadInt()));
                }
                else if (response == ServerResponse.PlayerIndex) {
                    PlayerIndex?.Invoke(this, new PlayerIndexEventArgs(ReadInt()));
                }
                else if (response == ServerResponse.Blinds) {
                    BlindsReceived?.Invoke(this, new BlindsReceivedEventArgs(ReadInt(), ReadInt()));
                }
                else if (response == ServerResponse.RequiredBet) {
                    RequiredBetReceived?.Invoke(this, new RequiredBetReceivedEventArgs(ReadInt()));
                }
                else if (response == ServerResponse.Showdown) {
                    int winnerCount = ReadInt();
                    List<int> winnerIndexes = new List<int>();

                    for (int i = 0; i < winnerCount; i++) {
                        winnerIndexes.Add(ReadInt());
                    }
                    
                    Showdown?.Invoke(this, new ShowdownEventArgs(winnerIndexes));
                }
                else if (response == ServerResponse.RoundFinished) {
                    RoundFinished?.Invoke(this, new RoundFinishedEventArgs());
                }

                responseCode = Session.Reader.Read();
            }
        }

        private void InitializeTable() {
            int seatIndex = ReadInt();
            int smallBlind = ReadInt();
            int buyIn = ReadInt();
            ServerJoinTableResponse response = (ServerJoinTableResponse) Session.Reader.Read();

            if (response == ServerJoinTableResponse.TableEmpty) {
                TableEmpty?.Invoke(this, new TableEmptyEventArgs(seatIndex, smallBlind, buyIn));
            }
            else if(response == ServerJoinTableResponse.TableNotEmpty) {
                string opponentUsername = ReadLine();
                int opponentStack = ReadInt();
                TableNotEmpty?.Invoke(this, new TableNotEmptyEventArgs(seatIndex, smallBlind, buyIn, opponentUsername, opponentStack));
            }
        }

        private int ReadInt() => int.Parse(ReadLine());
        private string ReadLine() => Session.Reader.ReadLine();
    }
}
