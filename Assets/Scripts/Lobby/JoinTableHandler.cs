using TMPro;
using UnityEngine;

namespace Lobby {
    public class JoinTableHandler : MonoBehaviour {
        [SerializeField] private TMP_Text messageText;

        private const int MinSmallBlindsToJoin = 20;

        public void JoinTable() {
            HideMessage();

            TableData tableData = GetComponent<TableData>();

            if (Session.ChipCount < tableData.SmallBlind * MinSmallBlindsToJoin) {
                DisplayMessage("You do not have enough chips to join this table.");
                return;
            }

            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.JoinTable);
            Session.Writer.WriteLine(tableData.Title);
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
            messageText.enabled = true;
            messageText.text = text;
        }

        private void HideMessage() {
            messageText.enabled = false;
        }
    }
}
