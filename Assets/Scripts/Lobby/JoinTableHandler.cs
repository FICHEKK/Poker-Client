using TMPro;
using UnityEngine;

namespace Lobby {
    public class JoinTableHandler : MonoBehaviour {
        private TMP_Text _messageText;

        public void Start() {
            _messageText = GameObject.Find("Message Text").GetComponent<TMP_Text>();
        }

        public void JoinTable() {
            HideMessage();
            
            string title = GetComponent<TableData>().Title;
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.JoinTable);
            Session.Writer.WriteLine(title);
            Session.Writer.Flush();

            int responseCode = Session.Reader.BaseStream.ReadByte();
            if (responseCode == -1) return;

            ServerResponse response = (ServerResponse) responseCode;
            if (response == ServerResponse.JoinTableSucceeded) {
                DisplayMessage("Joining table...");
                GetComponent<SceneLoader>().LoadScene();
            }
            else if (response == ServerResponse.JoinTableFailedTableFull) {
                DisplayMessage("Could not join: Table is full.");
            }
            else if (response == ServerResponse.JoinTableFailedTableDoesNotExist) {
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
            _messageText.enabled = true;
            _messageText.text = text;
        }

        private void HideMessage() {
            _messageText.enabled = false;
        }
    }
}
