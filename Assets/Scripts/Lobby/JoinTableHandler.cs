using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class JoinTableHandler : MonoBehaviour {
        [SerializeField] private Button joinTableButton;
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text messageText;

        public void JoinTable() {
            HideMessage();

            joinTableButton.interactable = false;
            DisplayMessage("Joining table...");

            string tableTitle = GetComponent<TableData>().Title;
            float buyIn = slider.value;

            new Thread(() => {
                Session.Writer.BaseStream.WriteByte((byte) ClientRequest.JoinTable);
                Session.Writer.WriteLine(tableTitle);
                Session.Writer.WriteLine(buyIn);

                int responseCode = Session.Reader.Read();
                if (responseCode == -1) return;

                ServerJoinTableResponse response = (ServerJoinTableResponse) responseCode;
            
                if (response == ServerJoinTableResponse.Success) {
                    MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
                }
                else if (response == ServerJoinTableResponse.TableFull) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Could not join: Table is full."));
                }
                else if (response == ServerJoinTableResponse.TableDoesNotExist) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Could not join: Table does not exist."));
                }
                else {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Unexpected error occurred. Please try again."));
                }
                
                MainThreadExecutor.Instance.Enqueue(() => joinTableButton.interactable = true);
            }).Start();
        }
        
        //----------------------------------------------------------------
        //                      Message display
        //----------------------------------------------------------------

        private void DisplayMessage(string text) {
            messageText.enabled = true;
            messageText.text = text;
        }

        private void HideMessage() {
            messageText.enabled = false;
        }
    }
}
