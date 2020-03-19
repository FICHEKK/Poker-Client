using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table.EventArguments;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table
{
    public class UserInterfaceManager : MonoBehaviour
    {
        [SerializeField] private Image[] communityCards;
        [SerializeField] private Transform dealerButton;

        [SerializeField] private StackDisplayer potStack;
        [SerializeField] private GameObject actionInterface;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Slider raiseSlider;
        [SerializeField] private TMP_Text callButtonText;
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
            
            HideCommunityCards();

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
            handler.RiverReceived += RiverReceivedEventHandler;
            handler.Showdown += ShowdownEventHandler;

            handler.RoundFinished += RoundFinishedEventHandler;
        }

        //----------------------------------------------------------------
        //                      Joining the table
        //----------------------------------------------------------------

        private void TableInitEventHandler(object sender, TableInitEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                dealerButton.transform.position = seats[e.DealerButtonIndex].DealerButton.position;
                
                int playerDataIndex = 0;
                
                for (int i = 0; i < e.MaxPlayers; i++)
                {
                    if (playerDataIndex >= e.Players.Count) break;
                    
                    if (e.Players[playerDataIndex].Index == i)
                    {
                        seats[i].SetUsername(e.Players[playerDataIndex].Username);
                        seats[i].SetStack(e.Players[playerDataIndex].Stack);
                        seats[i].BetStack.UpdateStack(e.Players[playerDataIndex].Bet);

                        if (e.Players[playerDataIndex].Folded)
                        {
                            seats[i].MarkAsWaiting();
                        }
                        else
                        {
                            seats[i].MarkAsPlaying();
                        }

                        playerDataIndex++;
                    }
                }

                if (e.PlayerIndex != -1)
                {
                    focusedSeatIndex = e.PlayerIndex;
                    seats[e.PlayerIndex].SetFocused(true);
                }

                for (int i = 0; i < e.CommunityCards.Count; i++)
                {
                    communityCards[i].enabled = true;
                    communityCards[i].sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.CommunityCards[i]);
                }

                smallBlind = e.SmallBlind;
                potStack.UpdateStack(e.Pot);
                
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
                if (e.Index == seatIndex)
                {
                    actionInterface.SetActive(true);
                    AudioManager.Instance.Play(Sound.Bell);
                }

                if (focusedSeatIndex >= 0)
                {
                    seats[focusedSeatIndex].SetFocused(false);
                }

                focusedSeatIndex = e.Index;
                seats[focusedSeatIndex].SetFocused(true);
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
                int raiseAmount = e.RaisedToAmount - seats[e.PlayerIndex].BetStack.Value;
                seats[e.PlayerIndex].PlaceChipsOnTable(raiseAmount);
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
                foreach (var index in e.JustJoinedPlayerIndexes)
                    seats[index].PlaceChipsOnTable(smallBlind * 2);
                
                dealerButton.transform.position = seats[e.DealerButtonIndex].DealerButton.position;
                
                if(seats[e.SmallBlindIndex].BetStack.Value == 0)
                    seats[e.SmallBlindIndex].PlaceChipsOnTable(smallBlind);
                
                if(seats[e.BigBlindIndex].BetStack.Value == 0)
                    seats[e.BigBlindIndex].PlaceChipsOnTable(smallBlind * 2);
            });

        private void RequiredBetReceivedEventHandler(object sender, RequiredBetReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                bool canCheck = e.RequiredCall == 0;
                
                checkButton.gameObject.SetActive(canCheck);
                callButton.gameObject.SetActive(!canCheck);

                if (!canCheck)
                {
                    requiredCallAmount = e.RequiredCall;
                    callButtonText.text = "Call " + requiredCallAmount;
                }
                
                SetRaiseSliderRange(e.MinRaise, e.MaxRaise);
            });

        //----------------------------------------------------------------
        //                      Round phase handlers
        //----------------------------------------------------------------

        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e) => 
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                StartCoroutine(DisplayFlop(e));
                StartCoroutine(AddBetsToPot());
            });
        
        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                StartCoroutine(DisplayCard(communityCards[3], e.Card));
                StartCoroutine(AddBetsToPot());
            });
        
        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) =>
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                StartCoroutine(DisplayCard(communityCards[4], e.Card));
                StartCoroutine(AddBetsToPot());
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
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                HideCommunityCards();
                potStack.UpdateStack(0);
            });

        //----------------------------------------------------------------
        //                      Helper methods
        //----------------------------------------------------------------

        private void SetRaiseSliderRange(int minRaise, int maxRaise)
        {
            raiseSlider.minValue = minRaise;
            raiseSlider.maxValue = maxRaise;
            raiseSlider.value = raiseSlider.minValue;
            UpdateRaiseButtonText();
        }

        public void UpdateRaiseButtonText()
        {
            raiseButtonText.text = "Raise to " + raiseSlider.value;
        }
        
        private void HideCommunityCards()
        {
            foreach (var card in communityCards)
            {
                card.enabled = false;
            }
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
            bool anyBetWasMoved = false;
            int potValue = potStack.Value;
            
            foreach (var seat in seats)
            {
                if (seat.BetStack.Value > 0)
                {
                    StartCoroutine(TranslateBetToPot(seat.BetStack));
                    potValue += seat.CurrentBet;
                    anyBetWasMoved = true;
                }
            }

            if (anyBetWasMoved)
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
        
        private IEnumerator DisplayFlop(FlopReceivedEventArgs e)
        {
            yield return StartCoroutine(DisplayCard(communityCards[0], e.Card1));
            yield return StartCoroutine(DisplayCard(communityCards[1], e.Card2));
            yield return StartCoroutine(DisplayCard(communityCards[2], e.Card3));
        }
        
        private static IEnumerator DisplayCard(Image cardImage, string card)
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
        }
    }
}
