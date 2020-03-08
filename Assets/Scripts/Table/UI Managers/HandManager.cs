using Table.EventArguments;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI_Managers
{
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private Image handCard1;
        [SerializeField] private Image handCard2;
        [SerializeField] private Image opponentCard1;
        [SerializeField] private Image opponentCard2;

        private void Awake()
        {
            var handler = GetComponentInParent<ServerConnectionHandler>();

            handler.HandReceived += HandReceivedEventHandler;
            handler.RoundFinished += RoundFinishedEventHandler;

            HideHandCards();
        }

        private void HandReceivedEventHandler(object sender, HandReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                handCard1.enabled = true;
                handCard2.enabled = true;
                handCard1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card1);
                handCard2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card2);

                opponentCard1.enabled = true;
                opponentCard2.enabled = true;
            });
        }

        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(HideHandCards);
        }

        private void HideHandCards()
        {
            handCard1.enabled = false;
            handCard2.enabled = false;
            opponentCard1.enabled = false;
            opponentCard2.enabled = false;
        }
    }
}
