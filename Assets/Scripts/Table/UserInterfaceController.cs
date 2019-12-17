using Table.EventArguments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table {
    public class UserInterfaceController : MonoBehaviour {
        [SerializeField] private GameObject myActionInterface;
        [SerializeField] private TMP_Text waitingForPlayerText;
        
        [SerializeField] private TMP_Text opponentUsernameText;
        [SerializeField] private TMP_Text opponentStackText;
        [SerializeField] private TMP_Text opponentActionText;
        [SerializeField] private TMP_Text opponentBetText;
        
        [SerializeField] private TMP_Text myStackText;
        [SerializeField] private TMP_Text myActionText;
        [SerializeField] private TMP_Text myBetText;

        [SerializeField] private TMP_Text potText;
        
        [SerializeField] private Image flopCard1;
        [SerializeField] private Image flopCard2;
        [SerializeField] private Image flopCard3;
        [SerializeField] private Image turnCard;
        [SerializeField] private Image riverCard;
        
        [SerializeField] private Image handCard1;
        [SerializeField] private Image handCard2;

        [SerializeField] private Image opponentCard1;
        [SerializeField] private Image opponentCard2;

        [SerializeField] private Slider raiseSlider;

        private ServerConnectionHandler handler;
        
        // General table information used by the UI controller
        public int SmallBlind { get; private set; }
        public int SeatIndex { get; private set; }
        public int MyStack { get; private set; }
        public int OpponentStack { get; private set; }
        
        public int MyCurrentBet { get; private set; }

        public void Start() {
            HideAllCards();
            HideOpponentInformation();
            HideOpponentAction();
            HideAllActions();
            HideAllBets();
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

        #region Event handlers

        private void TableEmptyEventHandler(object sender, TableEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                SeatIndex = e.SeatIndex;
                MyStack = e.BuyIn;
                SmallBlind = e.SmallBlind;
                myStackText.text = StringifyStack(e.BuyIn);
                
                raiseSlider.minValue = SmallBlind * 2;
                raiseSlider.maxValue = MyStack;

                ShowWaitingForPlayerText();
            });
        }

        private void TableNotEmptyEventHandler(object sender, TableNotEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                SeatIndex = e.SeatIndex;
                MyStack = e.BuyIn;
                SmallBlind = e.SmallBlind;
                myStackText.text = StringifyStack(e.BuyIn);
                
                raiseSlider.minValue = SmallBlind * 2;
                raiseSlider.maxValue = MyStack;

                OpponentStack = e.OpponentStack;
                ShowOpponentInformation(e.OpponentUsername, e.OpponentStack);
            });
        }

        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                OpponentStack = e.Stack;
                ShowOpponentInformation(e.Username, e.Stack);
                HideWaitingForPlayerText();
            });
        }
        
        private void PlayerLeftEventHandler(object sender, PlayerLeftEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(HideOpponentInformation);
            
        private void PlayerIndexEventHandler(object sender, PlayerIndexEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => myActionInterface.SetActive(e.Index == SeatIndex));
            
        private void PlayerCheckedEventHandler(object sender, PlayerCheckedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Check"));

        private void PlayerCalledEventHandler(object sender, PlayerCalledEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Call " + e.CallAmount));

        private void PlayerFoldedEventHandler(object sender, PlayerFoldedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Fold"));

        private void PlayerRaisedEventHandler(object sender, PlayerRaisedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                UpdateStack(e.PlayerIndex, -e.RaiseAmount);
                ShowAction(e.PlayerIndex, "Raise " + e.RaiseAmount);
            });
        }
            

        private void PlayerAllInEventHandler(object sender, PlayerAllInEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "All-In " + e.AllInAmount));

        private void BlindsReceivedEventHandler(object sender, BlindsReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                if (SeatIndex == e.SmallBlindIndex) {
                    myBetText.text = SmallBlind.ToString();
                    opponentBetText.text = (SmallBlind * 2).ToString();
                    MyCurrentBet = SmallBlind;
                }
                else {
                    myBetText.text = (SmallBlind * 2).ToString();
                    opponentBetText.text = SmallBlind.ToString();
                    MyCurrentBet = SmallBlind * 2;
                }
            });
        }
        
        private void RequiredBetReceivedEventHandler(object sender, RequiredBetReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                if (e.RequiredBet == MyCurrentBet) {
                    Debug.Log("You don't have to bet anything.");
                }
                else {
                    Debug.Log("Call " + (e.RequiredBet - MyCurrentBet) + "?");
                }
            });
        }

        #endregion
        
        #region Round phases
        
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
                turnCard.enabled = true;
                turnCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card);
            });
        }
        
        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                riverCard.enabled = true;
                riverCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card);
            });
        }
        
        private void ShowdownEventHandler(object sender, ShowdownEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => myActionInterface.SetActive(false));
        }
        
        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                HideAllCards();
                HideAllBets();
                HideAllActions();
            });
        }
        
        #endregion
        
        #region Showing UI

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

        #endregion

        #region Hiding UI

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

        #endregion

        #region Button handlers
        
        public void Check() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Check);
            Session.Writer.WriteLine(Session.Username);
        }
        
        public void Call() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Call);
            Session.Writer.WriteLine(Session.Username);
            
            //TODO add calling
        }
        
        public void Fold() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Fold);
            Session.Writer.WriteLine(Session.Username);
        }
        
        public void Raise() {
            bool isAllIn = (int) raiseSlider.value == (int) raiseSlider.maxValue;
            Session.Client.GetStream().WriteByte((byte) (isAllIn ? ClientRequest.AllIn : ClientRequest.Raise));
            Session.Writer.WriteLine(Session.Username);
            Session.Writer.WriteLine(raiseSlider.value);
        }

        public void AllIn() {
            raiseSlider.value = raiseSlider.maxValue;
        }

        #endregion
        
        /// <summary>
        /// Updates the stack value of the specified player.
        /// </summary>
        /// <param name="playerIndex">Index of the player whose stack is being updated.</param>
        /// <param name="stackDelta">
        /// The difference in stack.
        /// If placing blinds, calling or raising, this number is negative.
        /// If winning a round, this number is positive.
        /// </param>
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

        private string StringifyStack(int stack) {
            return "Chips: " + stack;
        }
    }
}
