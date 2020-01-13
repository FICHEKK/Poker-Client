using Table.EventArguments;
using TMPro;
using UnityEngine;

namespace Table.UI_Managers {
    public class WaitingForPlayerManager : MonoBehaviour {
        private const string WaitingText = "Waiting for a player to join...";
        
        [SerializeField] private TMP_Text waitingForPlayerText;
        
        private void Awake() {
            var handler = GetComponentInParent<ServerConnectionHandler>();
            
            handler.TableEmpty += TableEmptyEventHandler;
            handler.PlayerJoined += PlayerJoinedEventHandler;
            handler.PlayerLeft += PlayerLeftEventHandler;

            waitingForPlayerText.text = string.Empty;
        }

        private void TableEmptyEventHandler(object sender, TableEmptyEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => waitingForPlayerText.text = WaitingText);
        }
        
        private void PlayerJoinedEventHandler(object sender, PlayerJoinedEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => waitingForPlayerText.text = string.Empty);
        }
        
        private void PlayerLeftEventHandler(object sender, PlayerLeftEventArgs e) {
            MainThreadExecutor.Instance.Enqueue(() => waitingForPlayerText.text = WaitingText);
        }
    }
}
