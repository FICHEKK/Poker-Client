using System.Collections;
using Table.EventArguments;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI_Managers
{
    public class FlopManager : MonoBehaviour
    {
        [SerializeField] private Image flopCard1;
        [SerializeField] private Image flopCard2;
        [SerializeField] private Image flopCard3;

        private void Awake()
        {
            var handler = GetComponentInParent<ServerConnectionHandler>();

            handler.FlopReceived += FlopReceivedEventHandler;
            handler.RoundFinished += RoundFinishedEventHandler;

            HideFlopCards();
        }

        private void FlopReceivedEventHandler(object sender, FlopReceivedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(() => StartCoroutine(DisplayFlop(e)));
        }

        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e)
        {
            MainThreadExecutor.Instance.Enqueue(HideFlopCards);
        }

        private void HideFlopCards()
        {
            flopCard1.enabled = false;
            flopCard2.enabled = false;
            flopCard3.enabled = false;
        }

        private IEnumerator DisplayFlop(FlopReceivedEventArgs e)
        {
            yield return StartCoroutine(UserInterfaceManager.DisplayCard(flopCard1, e.Card1));
            yield return StartCoroutine(UserInterfaceManager.DisplayCard(flopCard2, e.Card2));
            yield return StartCoroutine(UserInterfaceManager.DisplayCard(flopCard3, e.Card3));
        }
    }
}
