using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class JoinTableHandler : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text messageText;

        public void JoinTable()
        {
            SendJoinDataToServer();
            ProcessServerResponse(Session.Reader.Read());
        }

        private void SendJoinDataToServer()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.JoinTable);
            Session.Writer.WriteLine(GetComponent<TableData>().Title);
            Session.Writer.WriteLine(slider.value);
        }
        
        private void ProcessServerResponse(int responseCode)
        {
            if (responseCode == -1)
            {
                DisplayMessage("Server connection error.");
                return;
            }

            switch ((ServerResponse) responseCode)
            {
                case ServerResponse.JoinTableSuccess:
                    GetComponent<SceneLoader>().LoadScene();
                    break;
                case ServerResponse.JoinTableTableFull:
                    DisplayMessage("Could not join: Table is full.");
                    break;
                case ServerResponse.JoinTableTableDoesNotExist:
                    DisplayMessage("Could not join: Table does not exist.");
                    break;
                default:
                    DisplayMessage("Unexpected error occurred. Please try again.");
                    break;
            }
        }

        //----------------------------------------------------------------
        //                      Message display
        //----------------------------------------------------------------

        private void DisplayMessage(string text)
        {
            messageText.enabled = true;
            messageText.text = text;
        }
    }
}
