using System.Threading;
using Table.EventArguments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table {
    
    public class UserInterfaceManager : MonoBehaviour {
        [SerializeField] private TMP_Text waitingForPlayerText;
        
        [SerializeField] private TMP_Text opponentUsernameText;
        [SerializeField] private TMP_Text opponentStackText;
        [SerializeField] private TMP_Text opponentActionText;
        [SerializeField] private TMP_Text opponentBetText;

        [SerializeField] private TMP_Text myStackText;
        [SerializeField] private TMP_Text myActionText;
        [SerializeField] private TMP_Text myBetText;
        [SerializeField] private TMP_Text potText;
        
        // Cards
        [SerializeField] private Image flopCard1;
        [SerializeField] private Image flopCard2;
        [SerializeField] private Image flopCard3;
        [SerializeField] private Image turnCard;
        [SerializeField] private Image riverCard;
        [SerializeField] private Image handCard1;
        [SerializeField] private Image handCard2;
        [SerializeField] private Image opponentCard1;
        [SerializeField] private Image opponentCard2;

        // Action
        [SerializeField] private GameObject myActionInterface;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Slider raiseSlider;
        
        private ServerConnectionHandler handler;
        
        // General table information used by the UI controller
        private int SmallBlind { get; set; }
        private int SeatIndex { get; set; }
        
        // Stack information
        private int MyStack { get; set; }
        private int OpponentStack { get; set; }
        
        // Bet and pot information
        private int MyCurrentBet { get; set; }
        private int OpponentCurrentBet { get; set; }
        private int CurrentPot { get; set; }

        private int RequiredCallAmount { get; set; }

        private void Start() {
            HideAllCards();
            HideOpponentInformation();
            HideOpponentAction();
            HideAllActions();
            HideAllBets();
            HidePot();
            HideWaitingForPlayerText();
            
            handler = new ServerConnectionHandler();
            
            handler.TableEmpty += TableEmptyEventHandler;
            handler.TableNotEmpty += TableNotEmptyEventHandler;
            
            handler.PlayerJoined += PlayerJoinedEventHandler;
            handler.PlayerLeft += PlayerLeftEventHandler;
            handler.PlayerIndex += PlayerIndexEventHandler;
            
            handler.PlayerChecked += PlayerCheckedEventHandler;
            handler.PlayerCalled += PlayerCalledEventHandler;
            handler.PlayerFolded += PlayerFoldedEventHandler;
            handler.PlayerRaised += PlayerRaisedEventHandler;
            handler.PlayerAllIn += PlayerAllInEventHandler;

            handler.BlindsReceived += BlindsReceivedEventHandler;
            handler.RequiredBetReceived += RequiredBetReceivedEventHandler;
            handler.HandReceived += HandReceivedEventHandler;
            handler.FlopReceived += FlopReceivedEventHandler;
            handler.TurnReceived += TurnReceivedEventHandler;
            handler.Showdown += ShowdownEventHandler;
            handler.RiverReceived += RiverReceivedEventHandler;
            
            handler.RoundFinished += RoundFinishedEventHandler;

            handler.Handle();
        }

        //----------------------------------------------------------------
        //                      Event handlers
        //----------------------------------------------------------------

        private void TableEmptyEventHandler(object sender, TableEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                SeatIndex = e.SeatIndex;
                MyStack = e.BuyIn;
                SmallBlind = e.SmallBlind;
                myStackText.text = StringifyStack(e.BuyIn);
                
                ShowWaitingForPlayerText();
                SetSliderRange(SmallBlind * 2, MyStack);
            });
        }

        private void TableNotEmptyEventHandler(object sender, TableNotEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                SeatIndex = e.SeatIndex;
                MyStack = e.BuyIn;
                SmallBlind = e.SmallBlind;
                myStackText.text = StringifyStack(e.BuyIn);

                OpponentStack = e.OpponentStack;
                ShowOpponentInformation(e.OpponentUsername, e.OpponentStack);
                ShowPot();
                SetSliderRange(SmallBlind * 2, MyStack);
            });
        }

        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                OpponentStack = e.Stack;
                ShowOpponentInformation(e.Username, e.Stack);
                ShowPot();
                HideWaitingForPlayerText();
            });
        }

        private void PlayerLeftEventHandler(object sender, PlayerLeftEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(HideOpponentInformation);
        }

        private void PlayerIndexEventHandler(object sender, PlayerIndexEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                myActionInterface.SetActive(e.Index == SeatIndex);
            });
        }

        private void PlayerCheckedEventHandler(object sender, PlayerCheckedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Check"));
        }

        private void PlayerCalledEventHandler(object sender, PlayerCalledEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                ShowAction(e.PlayerIndex, "Call");
                PlaceChipsOnTable(e.PlayerIndex, e.CallAmount);
            });
        }

        private void PlayerFoldedEventHandler(object sender, PlayerFoldedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Fold"));
        }

        private void PlayerRaisedEventHandler(object sender, PlayerRaisedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                ShowAction(e.PlayerIndex, "Raise");
                PlaceChipsOnTable(e.PlayerIndex, e.RaiseAmount);
            });
        }
        
        private void PlayerAllInEventHandler(object sender, PlayerAllInEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                ShowAction(e.PlayerIndex, "All-In");
                PlaceChipsOnTable(e.PlayerIndex, e.AllInAmount);
            });
        }

        private void BlindsReceivedEventHandler(object sender, BlindsReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                PlaceChipsOnTable(e.SmallBlindIndex, SmallBlind);
                PlaceChipsOnTable(e.BigBlindIndex, SmallBlind * 2);
            });
        }
        
        private void RequiredBetReceivedEventHandler(object sender, RequiredBetReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                if (e.RequiredBet == MyCurrentBet) {
                    checkButton.interactable = true;
                    callButton.interactable = false;
                    callButton.GetComponentInChildren<TMP_Text>().text = "Call";
                }
                else {
                    checkButton.interactable = false;
                    callButton.interactable = true;

                    RequiredCallAmount = e.RequiredBet - MyCurrentBet;
                    callButton.GetComponentInChildren<TMP_Text>().text = "Call " + RequiredCallAmount;

                    SetSliderRange(RequiredCallAmount * 2, MyStack);
                }
            });
        }

        //----------------------------------------------------------------
        //                      Round phase handlers
        //----------------------------------------------------------------
        
        private void HandReceivedEventHandler(object sender, HandReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                handCard1.enabled = true;
                handCard2.enabled = true;
                handCard1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card1);
                handCard2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card2);
                
                opponentCard1.enabled = true;
                opponentCard2.enabled = true;
            });
        }
        
        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                OnPhaseChange();
                flopCard1.enabled = true;
                flopCard2.enabled = true;
                flopCard3.enabled = true;
                flopCard1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card1);
                flopCard2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card2);
                flopCard3.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card3);
            });
        }
        
        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                OnPhaseChange();
                turnCard.enabled = true;
                turnCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card);
            });
        }
        
        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                OnPhaseChange();
                riverCard.enabled = true;
                riverCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card);
            });
        }
        
        private void ShowdownEventHandler(object sender, ShowdownEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                OnPhaseChange();

                if (e.WinnerIndexes.Count == 1) {
                    int winnerIndex = e.WinnerIndexes[0];
                    UpdateStack(winnerIndex, +CurrentPot);
                }
                else {
                    UpdateStack(0, +CurrentPot / 2);
                    UpdateStack(1, +CurrentPot / 2);
                }
                
                myActionInterface.SetActive(false);
            });
        }
        
        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                CurrentPot = 0;
                AddBetsToPot();

                HideAllCards();
                HideAllBets();
                HideAllActions();
            });
        }

        //----------------------------------------------------------------
        //                      Chip manipulation
        //----------------------------------------------------------------
        
        private void PlaceChipsOnTable(int playerIndex, int amount) {
            UpdateStack(playerIndex, -amount);
            
            if (playerIndex == SeatIndex) {
                MyCurrentBet += amount;
                myBetText.text = MyCurrentBet + " chips";
            }
            else {
                OpponentCurrentBet += amount;
                opponentBetText.text = OpponentCurrentBet + " chips";
            }
        }
        
        private void UpdateStack(int playerIndex, int stackDelta) {
            if (playerIndex == SeatIndex) {
                MyStack += stackDelta;
                myStackText.text = StringifyStack(MyStack);
            }
            else {
                OpponentStack += stackDelta;
                opponentStackText.text = StringifyStack(OpponentStack);
            }
        }
        
        private void AddBetsToPot() {
            CurrentPot += MyCurrentBet + OpponentCurrentBet;
            MyCurrentBet = 0;
            OpponentCurrentBet = 0;

            potText.text = "Pot: " + CurrentPot;
            myBetText.text = string.Empty;
            opponentBetText.text = string.Empty;
        }

        //----------------------------------------------------------------
        //                      Helper methods
        //----------------------------------------------------------------
        
        private string StringifyStack(int stack) {
            return "Chips: " + stack;
        }
        
        private void SetSliderRange(int minValue, int maxValue) {
            raiseSlider.minValue = minValue;
            raiseSlider.maxValue = maxValue;
        }
        
        private void OnPhaseChange() {
            HideAllActions();
            AddBetsToPot();
            SetSliderRange(SmallBlind * 2, MyStack);
            raiseSlider.value = raiseSlider.minValue;
        }
        
        //----------------------------------------------------------------
        //                         Showing UI
        //----------------------------------------------------------------
        
        private void ShowWaitingForPlayerText() {
            waitingForPlayerText.text = "Waiting for a player to join...";
        }
        
        private void ShowOpponentInformation(string username, int stack) {
            opponentUsernameText.text = username;
            opponentStackText.text = StringifyStack(stack);
        }

        private void ShowAction(int playerIndex, string action) {
            if (playerIndex == SeatIndex) {
                myActionText.text = action;
            }
            else {
                opponentActionText.text = action;
            }
        }

        private void ShowPot() {
            potText.text = "Pot: 0";
        }
        
        //----------------------------------------------------------------
        //                         Hiding UI
        //----------------------------------------------------------------

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
        
        private void HideWaitingForPlayerText() {
            waitingForPlayerText.text = string.Empty;
        }
        
        private void HideOpponentInformation() {
            opponentUsernameText.text = string.Empty;
            opponentStackText.text = string.Empty;
        }
        
        private void HideOpponentAction() {
            opponentActionText.text = string.Empty;
        }
        
        private void HideAllActions() {
            myActionText.text = string.Empty;
            opponentActionText.text = string.Empty;
        }

        private void HideAllBets() {
            myBetText.text = string.Empty;
            opponentBetText.text = string.Empty;
        }

        private void HidePot() {
            potText.text = string.Empty;
        }
        
        //----------------------------------------------------------------
        //                      Button handlers
        //----------------------------------------------------------------
        
        public void Check() {
            new Thread(() => {
                Session.Client.GetStream().WriteByte((byte) ClientRequest.Check);
            }).Start();
        }
        
        public void Call() {
            new Thread(() => {
                Session.Client.GetStream().WriteByte((byte) ClientRequest.Call);
                Session.Writer.WriteLine(RequiredCallAmount.ToString());
            }).Start();
        }
        
        public void Fold() {
            new Thread(() => {
                Session.Client.GetStream().WriteByte((byte) ClientRequest.Fold);
            }).Start();
        }
        
        public void Raise() {
            bool isAllIn = (int) raiseSlider.value == (int) raiseSlider.maxValue;
            float raiseAmount = raiseSlider.value;
            
            new Thread(() => {
                Session.Client.GetStream().WriteByte((byte) (isAllIn ? ClientRequest.AllIn : ClientRequest.Raise));
                Session.Writer.WriteLine(raiseAmount);
            }).Start();
        }

        public void AllIn() {
            raiseSlider.value = raiseSlider.maxValue;
        }
    }
}
