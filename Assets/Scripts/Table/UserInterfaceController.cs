using Table.EventArguments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table {
    public class UserInterfaceController : MonoBehaviour {
        [SerializeField] private TMP_Text waitingForPlayerText;
        [SerializeField] private TMP_Text opponentUsernameText;
        [SerializeField] private TMP_Text opponentChipCountText;
        [SerializeField] private TMP_Text opponentDecisionText;
        [SerializeField] private TMP_Text myChipCountText;
        [SerializeField] private TMP_Text myDecisionText;
        
        [SerializeField] private Image flopCard1;
        [SerializeField] private Image flopCard2;
        [SerializeField] private Image flopCard3;
        [SerializeField] private Image turnCard;
        [SerializeField] private Image riverCard;
        
        [SerializeField] private Image handCard1;
        [SerializeField] private Image handCard2;

        [SerializeField] private Image opponentCard1;
        [SerializeField] private Image opponentCard2;

        private ServerConnectionHandler handler;
        private int seatIndex;

        public void Start() {
            HideAllCards();
            HideOpponentInformation();
            HideOpponentDecision();
            HideMyDecision();
            HideWaitingForPlayerText();
            
            handler = new ServerConnectionHandler();
            
            handler.TableEmpty += TableEmptyEventHandler;
            handler.TableNotEmpty += TableNotEmptyEventHandler;
            
            handler.PlayerJoined += PlayerJoinedEventHandler;
            handler.PlayerLeft += PlayerLeftEventHandler;
            
            handler.PlayerChecked += PlayerCheckedEventHandler;
            handler.PlayerCalled += PlayerCalledEventHandler;
            handler.PlayerFolded += PlayerFoldedEventHandler;
            handler.PlayerRaised += PlayerRaisedEventHandler;
            handler.PlayerAllIn += PlayerAllInEventHandler;
            
            handler.HandReceived += HandReceivedEventHandler;
            handler.FlopReceived += FlopReceivedEventHandler;
            handler.TurnReceived += TurnReceivedEventHandler;
            handler.RiverReceived += RiverReceivedEventHandler;

            handler.Handle();
        }

        #region Event handlers

        private void TableEmptyEventHandler(object sender, TableEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                seatIndex = e.SeatIndex;
                myChipCountText.text = "Chips: " + e.BuyIn;
                ShowWaitingForPlayerText();
            });
        }

        private void TableNotEmptyEventHandler(object sender, TableNotEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                seatIndex = e.SeatIndex;
                myChipCountText.text = "Chips: " + e.BuyIn;
                ShowOpponentInformation(e.Username, e.ChipCount);
            });
        }
        
        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                ShowOpponentInformation(e.Username, e.ChipCount);
                HideWaitingForPlayerText();
            });
        }
        
        private void PlayerLeftEventHandler(object sender, PlayerLeftEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(HideOpponentInformation);
        }
        
        private void PlayerCheckedEventHandler(object sender, PlayerCheckedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowDecision(e.PlayerIndex, "Check"));
        }

        private void PlayerCalledEventHandler(object sender, PlayerCalledEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowDecision(e.PlayerIndex, "Call " + e.CallAmount));
        }

        private void PlayerFoldedEventHandler(object sender, PlayerFoldedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowDecision(e.PlayerIndex, "Fold"));
        }

        private void PlayerRaisedEventHandler(object sender, PlayerRaisedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowDecision(e.PlayerIndex, "Raise " + e.RaiseAmount));
        }

        private void PlayerAllInEventHandler(object sender, PlayerAllInEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowDecision(e.PlayerIndex, "All-In " + e.AllInAmount));
        }
        
        private void HandReceivedEventHandler(object sender, HandReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                ShowHandCards(e.Card1, e.Card2);
                ShowOpponentCards();
            });
        }
        
        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowFlopCards(e.Card1, e.Card2, e.Card3));
        }
        
        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowTurnCard(e.Card));
        }
        
        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowRiverCard(e.Card));
        }
        
        #endregion

        #region UI updating
        
        private void ShowHandCards(string card1, string card2) {
            handCard1.enabled = true;
            handCard2.enabled = true;
            handCard1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card1);
            handCard2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card2);
        }

        private void ShowOpponentCards() {
            opponentCard1.enabled = true;
            opponentCard2.enabled = true;
        }

        private void ShowFlopCards(string card1, string card2, string card3) {
            flopCard1.enabled = true;
            flopCard2.enabled = true;
            flopCard3.enabled = true;
            flopCard1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card1);
            flopCard2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card2);
            flopCard3.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card3);
        }

        private void ShowTurnCard(string card) {
            turnCard.enabled = true;
            turnCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card);
        }
        
        private void ShowRiverCard(string card) {
            riverCard.enabled = true;
            riverCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card);
        }

        private void HideAllCards() {
            handCard1.enabled = false;
            handCard2.enabled = false;

            opponentCard1.enabled = false;
            opponentCard2.enabled = false;
            
            flopCard1.enabled = false;
            flopCard2.enabled = false;
            flopCard3.enabled = false;
            turnCard.enabled = false;
            riverCard.enabled = false;
        }

        private void ShowWaitingForPlayerText() {
            waitingForPlayerText.enabled = true;
        }

        private void HideWaitingForPlayerText() {
            waitingForPlayerText.enabled = false;
        }

        private void ShowOpponentInformation(string username, int chipCount) {
            opponentUsernameText.enabled = true;
            opponentChipCountText.enabled = true;
            opponentUsernameText.text = username;
            opponentChipCountText.text = "Chips: " + chipCount;
        }

        private void HideOpponentInformation() {
            opponentUsernameText.enabled = false;
            opponentChipCountText.enabled = false;
        }

        private void ShowDecision(int playerIndex, string decision) {
            if (playerIndex == seatIndex) {
                myDecisionText.text = decision;
                opponentDecisionText.text = string.Empty;
            }
            else {
                myDecisionText.text = string.Empty;
                opponentDecisionText.text = decision;
            }
        }
        
        private void HideOpponentDecision() {
            opponentDecisionText.text = string.Empty;
        }

        private void HideMyDecision() {
            myDecisionText.text = string.Empty;
        }

        #endregion
    }
}
