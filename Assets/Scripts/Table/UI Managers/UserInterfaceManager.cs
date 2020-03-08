using System.Collections;
using System.Threading;
using Table.EventArguments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI_Managers
{
    public class UserInterfaceManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text opponentUsernameText;
        [SerializeField] private TMP_Text opponentStackText;
        [SerializeField] private TMP_Text opponentActionText;
        [SerializeField] private TMP_Text opponentBetText;

        [SerializeField] private TMP_Text myStackText;
        [SerializeField] private TMP_Text myActionText;
        [SerializeField] private TMP_Text myBetText;
        [SerializeField] private TMP_Text potText;

        [SerializeField] private GameObject myActionInterface;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Slider raiseSlider;

        [SerializeField] private TMP_Text raiseButtonText;

        private UserInterfaceData data;

        private void Awake()
        {
            data = GetComponent<UserInterfaceData>();
            data.potPositionOnTable = potText.transform.position;

            HideOpponentInformation();
            HideOpponentAction();
            HideAllActions();
            HideAllBets();
            HidePot();

            var handler = GetComponentInParent<ServerConnectionHandler>();

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
            handler.FlopReceived += FlopReceivedEventHandler;
            handler.TurnReceived += TurnReceivedEventHandler;
            handler.Showdown += ShowdownEventHandler;
            handler.RiverReceived += RiverReceivedEventHandler;

            handler.RoundFinished += RoundFinishedEventHandler;
        }

        //----------------------------------------------------------------
        //                      Joining the table
        //----------------------------------------------------------------

        private void TableEmptyEventHandler(object sender, TableEmptyEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                data.SeatIndex = e.SeatIndex;
                data.MyStack = e.BuyIn;
                data.SmallBlind = e.SmallBlind;
                myStackText.text = StringifyStack(e.BuyIn);

                SetSliderRange(data.SmallBlind * 2, data.MyStack);
                myActionInterface.SetActive(false);
            });
        }

        private void TableNotEmptyEventHandler(object sender, TableNotEmptyEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                data.SeatIndex = e.SeatIndex;
                data.MyStack = e.BuyIn;
                data.SmallBlind = e.SmallBlind;
                myStackText.text = StringifyStack(e.BuyIn);

                data.OpponentStack = e.OpponentStack;
                ShowOpponentInformation(e.OpponentUsername, e.OpponentStack);
                ShowPot();
                SetSliderRange(data.SmallBlind * 2, data.MyStack);
            });
        }

        //----------------------------------------------------------------
        //                      Event handlers
        //----------------------------------------------------------------

        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                if (e.Username == Session.Username) return;
                
                data.OpponentStack = e.Stack;
                ShowOpponentInformation(e.Username, e.Stack);
                ShowPot();
            });
        }

        private void PlayerLeftEventHandler(object sender, PlayerLeftEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(HideOpponentInformation);
        }

        private void PlayerIndexEventHandler(object sender, PlayerIndexEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() => myActionInterface.SetActive(e.Index == data.SeatIndex));
        }

        private void PlayerCheckedEventHandler(object sender, PlayerCheckedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Check"));
        }

        private void PlayerCalledEventHandler(object sender, PlayerCalledEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                ShowAction(e.PlayerIndex, "Call");
                PlaceChipsOnTable(e.PlayerIndex, e.CallAmount);
            });
        }

        private void PlayerFoldedEventHandler(object sender, PlayerFoldedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() => ShowAction(e.PlayerIndex, "Fold"));
        }

        private void PlayerRaisedEventHandler(object sender, PlayerRaisedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                ShowAction(e.PlayerIndex, "Raise");
                PlaceChipsOnTable(e.PlayerIndex, e.RaiseAmount);
            });
        }

        private void PlayerAllInEventHandler(object sender, PlayerAllInEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                ShowAction(e.PlayerIndex, "All-In");
                PlaceChipsOnTable(e.PlayerIndex, e.AllInAmount);
            });
        }

        private void BlindsReceivedEventHandler(object sender, BlindsReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                PlaceChipsOnTable(e.SmallBlindIndex, data.SmallBlind);
                PlaceChipsOnTable(e.BigBlindIndex, data.SmallBlind * 2);
            });
        }

        private void RequiredBetReceivedEventHandler(object sender, RequiredBetReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                checkButton.interactable = e.RequiredBet == data.MyCurrentBet;
                callButton.interactable = e.RequiredBet != data.MyCurrentBet;

                if (e.RequiredBet == data.MyCurrentBet)
                {
                    callButton.GetComponentInChildren<TMP_Text>().text = "Call";
                }
                else
                {
                    data.RequiredCallAmount = e.RequiredBet - data.MyCurrentBet;
                    callButton.GetComponentInChildren<TMP_Text>().text = "Call " + data.RequiredCallAmount;

                    SetSliderRange(data.RequiredCallAmount * 2, data.MyStack);
                }
            });
        }

        //----------------------------------------------------------------
        //                      Round phase handlers
        //----------------------------------------------------------------

        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(OnPhaseChange);
        }

        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(OnPhaseChange);
        }

        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(OnPhaseChange);
        }

        private void ShowdownEventHandler(object sender, ShowdownEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                myActionInterface.SetActive(false);
                OnPhaseChange();

                if (e.WinnerIndexes.Count == 1)
                {
                    StartCoroutine(TranslatePotToWinningPlayer(e.WinnerIndexes[0]));
                }
                else
                {
                    UpdateStack(0, +data.CurrentPot / 2);
                    UpdateStack(1, +data.CurrentPot / 2);
                }
            });
        }

        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                data.CurrentPot = 0;
                AddBetsToPot();

                HideAllBets();
                HideAllActions();
            });
        }

        //----------------------------------------------------------------
        //                      Chip manipulation
        //----------------------------------------------------------------

        private void PlaceChipsOnTable(int playerIndex, int amount)
        {
            UpdateStack(playerIndex, -amount);

            if (playerIndex == data.SeatIndex)
            {
                data.MyCurrentBet += amount;
                myBetText.text = data.MyCurrentBet + " chips";
            }
            else
            {
                data.OpponentCurrentBet += amount;
                opponentBetText.text = data.OpponentCurrentBet + " chips";
            }
        }

        private void UpdateStack(int playerIndex, int stackDelta)
        {
            if (playerIndex == data.SeatIndex)
            {
                data.MyStack += stackDelta;
                myStackText.text = StringifyStack(data.MyStack);
            }
            else
            {
                data.OpponentStack += stackDelta;
                opponentStackText.text = StringifyStack(data.OpponentStack);
            }
        }

        private void AddBetsToPot()
        {
            data.CurrentPot += data.MyCurrentBet + data.OpponentCurrentBet;
            data.MyCurrentBet = 0;
            data.OpponentCurrentBet = 0;

            potText.text = "Pot: " + data.CurrentPot;
            myBetText.text = string.Empty;
            opponentBetText.text = string.Empty;
        }

        //----------------------------------------------------------------
        //                      Helper methods
        //----------------------------------------------------------------

        private static string StringifyStack(int stack)
        {
            return "Chips: " + stack;
        }

        private void SetSliderRange(int minValue, int maxValue)
        {
            raiseSlider.minValue = minValue;
            raiseSlider.maxValue = maxValue;
            raiseSlider.value = raiseSlider.minValue;
            UpdateRaiseButtonText();
        }

        private void OnPhaseChange()
        {
            HideAllActions();
            AddBetsToPot();
            SetSliderRange(data.SmallBlind * 2, data.MyStack);
        }

        public void UpdateRaiseButtonText()
        {
            raiseButtonText.text = "Raise " + raiseSlider.value;
        }

        //----------------------------------------------------------------
        //                         Showing UI
        //----------------------------------------------------------------

        private void ShowOpponentInformation(string username, int stack)
        {
            opponentUsernameText.text = username;
            opponentStackText.text = StringifyStack(stack);
        }

        private void ShowAction(int playerIndex, string action)
        {
            if (playerIndex == data.SeatIndex)
            {
                myActionText.text = action;
            }
            else
            {
                opponentActionText.text = action;
            }
        }

        private void ShowPot()
        {
            potText.text = "Pot: 0";
        }

        //----------------------------------------------------------------
        //                         Hiding UI
        //----------------------------------------------------------------

        private void HideOpponentInformation()
        {
            opponentUsernameText.text = string.Empty;
            opponentStackText.text = string.Empty;
        }

        private void HideOpponentAction()
        {
            opponentActionText.text = string.Empty;
        }

        private void HideAllActions()
        {
            myActionText.text = string.Empty;
            opponentActionText.text = string.Empty;
        }

        private void HideAllBets()
        {
            myBetText.text = string.Empty;
            opponentBetText.text = string.Empty;
        }

        private void HidePot()
        {
            potText.text = string.Empty;
        }

        //----------------------------------------------------------------
        //                 Moving pot to winning player
        //----------------------------------------------------------------

        private IEnumerator TranslatePotToWinningPlayer(int winnerIndex)
        {
            Vector3 destination;

            if (winnerIndex == data.SeatIndex)
            {
                destination = myStackText.transform.position;
            }
            else
            {
                destination = opponentStackText.transform.position;
            }

            yield return StartCoroutine(TranslateGameObject(potText.transform, destination, 0.5f));

            potText.text = string.Empty;
            potText.transform.position = data.potPositionOnTable;
            UpdateStack(winnerIndex, +data.CurrentPot);
        }

        private IEnumerator TranslateGameObject(Transform subject, Vector3 destination, float animationDuration)
        {
            int timeSlices = 50;
            Vector3 direction = destination - subject.transform.position;
            direction.z = 0f;
            direction /= timeSlices;

            for (int i = 0; i < timeSlices; i++)
            {
                subject.Translate(direction);
                yield return new WaitForSeconds(animationDuration / timeSlices);
            }
        }

        //----------------------------------------------------------------
        //                      Button handlers
        //----------------------------------------------------------------

        public void Check()
        {
            myActionInterface.SetActive(false);
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Check);
        }

        public void Call()
        {
            myActionInterface.SetActive(false);
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Call);
            Session.Writer.WriteLine(data.RequiredCallAmount.ToString());
        }

        public void Fold()
        {
            myActionInterface.SetActive(false);
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Fold);
        }

        public void Raise()
        {
            myActionInterface.SetActive(false);
            bool isAllIn = (int) raiseSlider.value == (int) raiseSlider.maxValue;

            Session.Client.GetStream().WriteByte((byte) (isAllIn ? ClientRequest.AllIn : ClientRequest.Raise));
            Session.Writer.WriteLine(raiseSlider.value);
        }

        public void AllIn()
        {
            raiseSlider.value = raiseSlider.maxValue;
        }

        public void Leave()
        {
            // TODO
            // Send leaving flag to server
            // Jump to lobby
            // On the other side, update UI that the opponent left and get all the money in the pot.
        }
    }
}
