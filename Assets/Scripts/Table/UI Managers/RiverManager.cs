using Table.EventArguments;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI_Managers {
    public class RiverManager : MonoBehaviour {
        [SerializeField] private Image riverCard;
        
        private void Awake() {
            var handler = GetComponentInParent<ServerConnectionHandler>();
            
            handler.RiverReceived += RiverReceivedEventHandler;
            handler.RoundFinished += RoundFinishedEventHandler;
            
            HideRiverCard();
        }

        private void RiverReceivedEventHandler(object sender, RiverReceivedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => {
                riverCard.enabled = true;
                riverCard.sprite = Resources.Load<Sprite>("Sprites/Cards/" + e.Card);
            });
        }
        
        private void RoundFinishedEventHandler(object sender, RoundFinishedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(HideRiverCard);
        }

        private void HideRiverCard() {
            riverCard.enabled = false;
        }
    }
}
