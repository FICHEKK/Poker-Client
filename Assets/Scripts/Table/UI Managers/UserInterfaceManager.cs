using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private int requiredCallAmount;

        private int focusedSeatIndex = -1;

        private void Awake()
        {
            foreach (var seat in seats)
            {
                seat.MarkAsEmpty();
            }

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
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                foreach (var seat in seats.Where(seat => !seat.IsEmpty))
                {
                    seat.MarkAsPlaying();
                    seat.HideCards();
                }
                
                seats[seatIndex].ShowCards(e.Card1, e.Card2);
                AudioManager.Instance.Play(Sound.DeckShuffle);
            });

        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e) =>
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
        
        //----------------------------------------------------------------
        //                      Player actions
        //----------------------------------------------------------------

        private void PlayerCheckedEventHandler(object sender, PlayerCheckedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                AudioManager.Instance.Play(Sound.Check);
            });

        private void PlayerCalledEventHandler(object sender, PlayerCalledEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                seats[e.PlayerIndex].PlaceChipsOnTable(e.CallAmount);
                AudioManager.Instance.Play(Sound.Call);
            });

        private void PlayerFoldedEventHandler(object sender, PlayerFoldedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                seats[e.PlayerIndex].MarkAsWaiting();
                AudioManager.Instance.Play(Sound.Fold);
            });

        private void PlayerRaisedEventHandler(object sender, PlayerRaisedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                seats[e.PlayerIndex].PlaceChipsOnTable(e.RaiseAmount);
                AudioManager.Instance.Play(Sound.Raise);
            });

        private void PlayerAllInEventHandler(object sender, PlayerAllInEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                seats[e.PlayerIndex].PlaceChipsOnTable(e.AllInAmount);
                AudioManager.Instance.Play(Sound.AllIn);
            });

        private void BlindsReceivedEventHandler(object sender, BlindsReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                seats[e.SmallBlindIndex].PlaceChipsOnTable(smallBlind);
                seats[e.BigBlindIndex].PlaceChipsOnTable(smallBlind * 2);
            });

        private void RequiredBetReceivedEventHandler(object sender, RequiredBetReceivedEventArgs e) =>
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

        //----------------------------------------------------------------
        //                      Round phase handlers
        //----------------------------------------------------------------

        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e) => 
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                StartCoroutine(AddBetsToPot());
                SetSliderRange(smallBlind * 2, seats[seatIndex].Stack);
            });
        
        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                StartCoroutine(AddBetsToPot());
                SetSliderRange(smallBlind * 2, seats[seatIndex].Stack);
            });
        
        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                StartCoroutine(AddBetsToPot());
                SetSliderRange(smallBlind * 2, seats[seatIndex].Stack);
            });

        private void ShowdownEventHandler(object sender, ShowdownEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                actionInterface.SetActive(false);

                if (e.WinnerIndexes.Count == 1)
                {
                    StartCoroutine(TranslatePotToWinningPlayer(e.WinnerIndexes[0]));
                }
                else
                {
                    foreach (int winnerIndex in e.WinnerIndexes)
                    {
                        seats[winnerIndex].GiveChips(potStack.Value / e.WinnerIndexes.Count);
                    }
                }
            });
        }

        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() => potStack.UpdateStack(0));

        //----------------------------------------------------------------
        //                      Helper methods
        //----------------------------------------------------------------

        private void SetSliderRange(int minValue, int maxValue)
        {
            raiseSlider.minValue = minValue;
            raiseSlider.maxValue = maxValue;
            raiseSlider.value = raiseSlider.minValue;
            UpdateRaiseButtonText();
        }

        public void UpdateRaiseButtonText()
        {
            raiseButtonText.text = "Raise " + raiseSlider.value;
        }

        //----------------------------------------------------------------
        //                         Coroutine
        //----------------------------------------------------------------

        private IEnumerator TranslatePotToWinningPlayer(int winnerIndex)
        {
            yield return StartCoroutine(AddBetsToPot());
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(TranslateGameObject(potStack.transform, seats[winnerIndex].transform.position, 0.5f));
            
            seats[winnerIndex].GiveChips(potStack.Value);
            AudioManager.Instance.Play(Sound.Raise);
            
            potStack.UpdateStack(0);
            potStack.transform.position = potStack.OriginalPosition;
        }
        
        private IEnumerator AddBetsToPot()
        {
            bool atleastOneBetWasMoved = false;
            int potValue = potStack.Value;
            
            foreach (var seat in seats)
            {
                if (seat.BetStack.Value > 0)
                {
                    StartCoroutine(TranslateBetToPot(seat.BetStack));
                    potValue += seat.CurrentBet;
                    atleastOneBetWasMoved = true;
                }
            }

            if (atleastOneBetWasMoved)
            {
                yield return new WaitForSeconds(0.9f);

                potStack.UpdateStack(potValue);
                AudioManager.Instance.Play(Sound.Raise);
            }
        }

        private IEnumerator TranslateBetToPot(StackDisplayer bet)
        {
            yield return StartCoroutine(TranslateGameObject(bet.transform, potStack.transform.position, 0.5f));
            
            bet.UpdateStack(0);
            bet.transform.position = bet.OriginalPosition;
        }

        private static IEnumerator TranslateGameObject(Transform subject, Vector3 destination, float animationDuration)
        {
            int timeSlices = 50;
            Vector3 direction = (destination - subject.transform.position) / timeSlices;

            for (int i = 0; i < timeSlices; i++)
            {
                subject.Translate(direction);
                yield return new WaitForSeconds(animationDuration / timeSlices);
            }
        }
        
        public static IEnumerator DisplayCard(Image cardImage, string card)
        {
            cardImage.sprite = Resources.Load<Sprite>("Sprites/Cards/Back");
            cardImage.enabled = true;
            AudioManager.Instance.Play(Sound.CardFlip);

            const int timeSlices = 15;
            const float rotationPerFrame = 90f / timeSlices;
            
            for (int i = 0; i < timeSlices; i++)
            {
                cardImage.transform.Rotate(0f, rotationPerFrame, 0f);
                yield return null;
            }
            
            cardImage.sprite = Resources.Load<Sprite>("Sprites/Cards/" + card);

            for (int i = 0; i < timeSlices; i++)
            {
                cardImage.transform.Rotate(0f, -rotationPerFrame, 0f);
                yield return null;
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
