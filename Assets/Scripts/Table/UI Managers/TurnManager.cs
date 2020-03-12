using Table.EventArguments;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI_Managers
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private Image turnCard;

        private void Awake()
        {
            var handler = GetComponentInParent<ServerConnectionHandler>();

            handler.TurnReceived += TurnReceivedEventHandler;
            handler.RoundFinished += RoundFinishedEventHandler;

            HideTurnCard();
        }

        private void TurnReceivedEventHandler(object sender, TurnReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() => StartCoroutine(UserInterfaceManager.DisplayCard(turnCard, e.Card)));
        }

        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(HideTurnCard);
        }

        private void HideTurnCard()
        {
            turnCard.enabled = false;
        }
    }
}
