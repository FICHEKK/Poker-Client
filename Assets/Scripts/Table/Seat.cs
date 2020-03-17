using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table
{
    public class Seat : MonoBehaviour
    {
        [SerializeField] private Image card1;
        [SerializeField] private Image card2;
        [SerializeField] private Image frame;
        [SerializeField] private Image frameBorder;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text stackText;
        [SerializeField] private StackDisplayer betStack;
        [SerializeField] private Transform dealerButton;

        public int Stack { get; private set; }
        public int CurrentBet => betStack.Value;
        public bool IsEmpty => stackText.text == string.Empty;
        public StackDisplayer BetStack => betStack;
        public Transform DealerButton => dealerButton;

        public void SetUsername(string username)
        {
            usernameText.text = username;
        }

        public void SetStack(int stack)
        {
            Stack = stack;
            stackText.text = stack.ToString();
        }

        public void PlaceChipsOnTable(int amount)
        {
            SetStack(Stack - amount);
            betStack.UpdateStack(betStack.Value + amount);
        }

        public void GiveChips(int amount)
        {
            SetStack(Stack + amount);
        }

        public void ShowCards(string c1, string c2)
        {
            ToggleCards(true);
            card1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + c1);
            card2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + c2);
        }

        public void HideCards()
        {
            ShowCards("Back", "Back");
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
            frameBorder.enabled = focused;
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
