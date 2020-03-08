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
            MainThreadExecutor.Instance.Enqueue(() =>
            {
                flopCard1.enabled = true;
                flopCard2.enabled = true;
                flopCard3.enabled = true;
                flopCard1.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card1);
                flopCard2.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card2);
                flopCard3.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card3);
            });
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
    }
}
