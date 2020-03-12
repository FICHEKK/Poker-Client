using System.Collections;
using System.Collections.Generic;
using Table.EventArguments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI_Managers
{
    public class UserInterfaceManager : MonoBehaviour
    {
        [SerializeField] private StackDisplayer potStack;
        [SerializeField] private GameObject actionInterface;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Slider raiseSlider;
        [SerializeField] private TMP_Text raiseButtonText;
        
        [SerializeField] private List<Seat> seats;

        private int smallBlind;
        private int seatIndex;
        private int currentPot;
        private int requiredCallAmount;
        private Vector3 potPositionOnTable;

        private int focusedSeatIndex = -1;

        private void Awake()
        {
            foreach (var seat in seats)
            {
                seat.MarkAsEmpty();
            }
            
            potPositionOnTable = potStack.transform.position;

            var handler = GetComponentInParent<ServerConnectionHandler>();

            handler.TableInit += TableInitEventHandler;
            handler.HandReceived += HandReceivedEventHandler;
            
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

        private void TableInitEventHandler(object sender, TableInitEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                smallBlind = e.SmallBlind;

                int playerDataIndex = 0;
                
                for (int i = 0; i < e.MaxPlayers; i++)
                {
                    if (playerDataIndex >= e.Players.Count) break;
                    
                    if (e.Players[playerDataIndex].Index == i)
                    {
                        seats[i].SetUsername(e.Players[playerDataIndex].Username);
                        seats[i].SetStack(e.Players[playerDataIndex].Stack);
                        playerDataIndex++;
                    }
                }

                SetSliderRange(smallBlind * 2, seats[seatIndex].Stack);
                actionInterface.SetActive(false);
            });
        }

        //----------------------------------------------------------------
        //                      Event handlers
        //----------------------------------------------------------------
        
        private void HandReceivedEventHandler(object sender, HandReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => seats[seatIndex].ShowCards(e.Card1, e.Card2));

        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                if (Session.Username == e.Username)
                {
                    seatIndex = e.Index;
                }
                
                seats[e.Index].SetUsername(e.Username);
                seats[e.Index].SetStack(e.Stack);
                seats[e.Index].MarkAsWaiting();
            });
        }

        private void PlayerLeftEventHandler(object sender, PlayerLeftEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => seats[e.Index].MarkAsEmpty());

        private void PlayerIndexEventHandler(object sender, PlayerIndexEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                actionInterface.SetActive(e.Index == seatIndex);

                if (focusedSeatIndex >= 0)
                {
                    seats[focusedSeatIndex].MarkAsPlaying();
                }

                focusedSeatIndex = e.Index;
                seats[focusedSeatIndex].MarkAsFocused();
            });

        private void PlayerCheckedEventHandler(object sender, PlayerCheckedEventArgs e)
        {
            // Change seat frame color to indicate checking
        }

        private void PlayerCalledEventHandler(object sender, PlayerCalledEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => seats[e.PlayerIndex].PlaceChipsOnTable(e.CallAmount));

        private void PlayerFoldedEventHandler(object sender, PlayerFoldedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => seats[e.PlayerIndex].MarkAsWaiting());

        private void PlayerRaisedEventHandler(object sender, PlayerRaisedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => seats[e.PlayerIndex].PlaceChipsOnTable(e.RaiseAmount));

        private void PlayerAllInEventHandler(object sender, PlayerAllInEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => seats[e.PlayerIndex].PlaceChipsOnTable(e.AllInAmount));

        private void BlindsReceivedEventHandler(object sender, BlindsReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                seats[e.SmallBlindIndex].PlaceChipsOnTable(smallBlind);
                seats[e.BigBlindIndex].PlaceChipsOnTable(smallBlind * 2);
            });
        }

        private void RequiredBetReceivedEventHandler(object sender, RequiredBetReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                bool canCheck = e.RequiredBet == 0;
                
                checkButton.interactable = canCheck;
                callButton.interactable = !canCheck;

                if (canCheck)
                {
                    callButton.GetComponentInChildren<TMP_Text>().text = "Call";
                }
                else
                {
                    requiredCallAmount = e.RequiredBet;
                    callButton.GetComponentInChildren<TMP_Text>().text = "Call " + requiredCallAmount;

                    SetSliderRange(requiredCallAmount * 2, seats[seatIndex].Stack);
                }
            });
        }

        //----------------------------------------------------------------
        //                      Round phase handlers
        //----------------------------------------------------------------

        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e) => MainThreadExecutor.Instance.Enqueue(OnPhaseChange);
        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e) => MainThreadExecutor.Instance.Enqueue(OnPhaseChange);
        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) => MainThreadExecutor.Instance.Enqueue(OnPhaseChange);

        private void ShowdownEventHandler(object sender, ShowdownEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                actionInterface.SetActive(false);
                OnPhaseChange();

                if (e.WinnerIndexes.Count == 1)
                {
                    StartCoroutine(TranslatePotToWinningPlayer(e.WinnerIndexes[0]));
                }
                else
                {
                    foreach (int winnerIndex in e.WinnerIndexes)
                    {
                        seats[winnerIndex].GiveChips(currentPot / e.WinnerIndexes.Count);
                    }
                }
            });
        }

        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                currentPot = 0;
                potStack.UpdateStack(0);
            });
        }

        //----------------------------------------------------------------
        //                      Helper methods
        //----------------------------------------------------------------

        private void SetSliderRange(int minValue, int maxValue)
        {
            raiseSlider.minValue = minValue;
            raiseSlider.maxValue = maxValue;
            raiseSlider.value = raiseSlider.minValue;
            raiseButtonText.text = "Raise " + raiseSlider.value;
        }

        private void OnPhaseChange()
        {
            AddBetsToPot();
            SetSliderRange(smallBlind * 2, seats[seatIndex].Stack);
        }

        private void AddBetsToPot()
        {
            foreach (var seat in seats)
            {
                currentPot += seat.CurrentBet;
                seat.HideBet();
            }
            
            potStack.UpdateStack(currentPot);
        }

        //----------------------------------------------------------------
        //                 Moving pot to winning player
        //----------------------------------------------------------------

        private IEnumerator TranslatePotToWinningPlayer(int winnerIndex)
        {
            Vector3 destination = seats[winnerIndex].transform.position;

            yield return StartCoroutine(TranslateGameObject(potStack.transform, destination, 0.5f));

            potStack.UpdateStack(0);
            potStack.transform.position = potPositionOnTable;
            seats[winnerIndex].GiveChips(currentPot);
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
            actionInterface.SetActive(false);
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Check);
        }

        public void Call()
        {
            actionInterface.SetActive(false);
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Call);
            Session.Writer.WriteLine(requiredCallAmount.ToString());
        }

        public void Fold()
        {
            actionInterface.SetActive(false);
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Fold);
        }

        public void Raise()
        {
            actionInterface.SetActive(false);
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
            Session.Client.GetStream().WriteByte((byte) ClientRequest.LeaveTable);
            GetComponent<SceneLoader>().LoadScene();
        }
    }
}
