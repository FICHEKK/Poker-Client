using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class JoinTableHandler : MonoBehaviour {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text messageText;

        public void JoinTable() {
            HideMessage();

            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.JoinTable);
            Session.Writer.WriteLine(GetComponent<TableData>().Title);
            Session.Writer.WriteLine(Session.Username);
            Session.Writer.WriteLine(slider.value);

            int responseCode = Session.Reader.Read();
            if (responseCode == -1) return;

            ServerJoinTableResponse response = (ServerJoinTableResponse) responseCode;
            
            if (response == ServerJoinTableResponse.Success) {
                DisplayMessage("Joining table...");
                GetComponent<SceneLoader>().LoadScene();
            }
            else if (response == ServerJoinTableResponse.TableFull) {
                DisplayMessage("Could not join: Table is full.");
            }
            else if (response == ServerJoinTableResponse.TableDoesNotExist) {
                DisplayMessage("Could not join: Table does not exist.");
            }
            else {
                DisplayMessage("Unexpected error occurred. Please try again.");
            }
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
