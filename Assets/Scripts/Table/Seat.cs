using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table
{
    public class Seat : MonoBehaviour
    {
        private const float ChatMessageHideDelayInSeconds = 5f;
        private float lastMessageSentTime;
        
        [SerializeField] private Image card1;
        [SerializeField] private Image card2;
        [SerializeField] private Image frame;
        [SerializeField] private Image frameBorder;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text stackText;
        [SerializeField] private TMP_Text chatText;
        public StackDisplayer BetStack;
        public Transform DealerButton;

        public int Stack { get; private set; }
        public int CurrentBet => BetStack.Value;
        public bool IsEmpty => stackText.text == string.Empty;
        public Image Card1 => card1;
        public Image Card2 => card2;

        private Coroutine animateFrameBorderCoroutine;

        public void SetUsername(string username)
        {
            usernameText.text = username;
        }

        public void SetStack(int stack)
        {
            Stack = stack;
            stackText.text = stack.ToString();
        }

        public void SetChatMessage(string message)
        {
            chatText.text = message;
            lastMessageSentTime = Time.time;
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(chatText.text)) return;

            if (Time.time > lastMessageSentTime + ChatMessageHideDelayInSeconds)
            {
                chatText.text = string.Empty;
            }
        }

        public void PlaceChipsOnTable(int amount)
        {
            SetStack(Stack - amount);
            BetStack.UpdateStack(BetStack.Value + amount);
        }

        public void GiveChips(int amount)
        {
            SetStack(Stack + amount);
        }

        public void HideCards()
        {
            ToggleCards(true);
            card1.sprite = Resources.Load<Sprite>("Sprites/Cards/Back");
            card2.sprite = Resources.Load<Sprite>("Sprites/Cards/Back");
        }

        //----------------------------------------------------------------
        //                          Seat states
        //----------------------------------------------------------------
        
        public void MarkAsEmpty()
        {
            ToggleCards(false);
            SetFrameColor(Color.white);
            SetFrameAlpha(0.2f);
            SetUsername("Empty");
            stackText.text = string.Empty;
            frameBorder.enabled = false;
        }
        
        public void MarkAsWaiting()
        {
            ToggleCards(false);
            SetFrameColor(Color.white);
            SetFrameAlpha(0.5f);
            frameBorder.enabled = false;
        }

        public void MarkAsPlaying()
        {
            ToggleCards(true);
            SetFrameColor(Color.white);
            SetFrameAlpha(1f);
            frameBorder.enabled = false;
        }

        public void SetFocused(bool focused)
        {
            if (animateFrameBorderCoroutine != null)
                StopCoroutine(animateFrameBorderCoroutine);
            
            frameBorder.enabled = focused;
            
            if (focused)
            {
                frameBorder.fillAmount = 1;
                animateFrameBorderCoroutine = StartCoroutine(AnimateFrameBorder());
            }
        }

        private IEnumerator AnimateFrameBorder()
        {
            var startingColor = Color.green;
            var endingColor = Color.red;
            const float decreasePerFrame = 1 / TableConstant.PlayerTurnDuration;
            
            while (frameBorder.enabled && frameBorder.fillAmount > 0)
            {
                frameBorder.fillAmount -= decreasePerFrame * Time.deltaTime;
                frameBorder.color = Color.Lerp(endingColor, startingColor, frameBorder.fillAmount);
                yield return null;
            }
        }

        //----------------------------------------------------------------
        //                      Helper methods
        //----------------------------------------------------------------

        private void SetFrameAlpha(float alpha)
        {
            var tempColor = frame.color;
            tempColor.a = alpha;
            frame.color = tempColor;
        }

        private void SetFrameColor(Color color)
        {
            frame.color = color;
        }

        private void ToggleCards(bool shouldShow)
        {
            card1.enabled = shouldShow;
            card2.enabled = shouldShow;
        }
    }
}
