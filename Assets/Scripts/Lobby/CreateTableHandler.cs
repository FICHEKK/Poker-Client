using TMPro;
using UnityEngine;

namespace Lobby
{
    public class CreateTableHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField tableTitleInputField;
        [SerializeField] private TMP_InputField smallBlindInputField;
        [SerializeField] private TMP_InputField maxPlayersInputField;
        [SerializeField] private TMP_Text messageText;

        private const int MinSmallBlind = 1;
        private const int MaxSmallBlind = 1_000_000;

        private const int MinNumberOfPlayers = 2;
        private const int MaxNumberOfPlayers = 10;

        public void CreateTable()
        {
            if (!IsLoginFormValid()) return;

            SendTableDataToServer();
            ProcessServerResponse(Session.Reader.Read());
        }

        private void SendTableDataToServer()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.CreateTable);
            Session.Writer.WriteLine(tableTitleInputField.text);
            Session.Writer.WriteLine(smallBlindInputField.text);
            Session.Writer.WriteLine(maxPlayersInputField.text);
        }

        private void ProcessServerResponse(int responseCode)
        {
            if (responseCode == -1)
            {
                DisplayMessage("Server connection error.");
                return;
            }
            
            switch ((ServerCreateTableResponse) responseCode)
            {
                case ServerCreateTableResponse.Success:
                    DisplayMessage("Table \"" + tableTitleInputField.text + "\" was created!");
                    break;
                case ServerCreateTableResponse.TitleTaken:
                    DisplayMessage("Table \"" + tableTitleInputField.text + "\" already exists.");
                    break;
                default:
                    DisplayMessage("Invalid server response. Please try again.");
                    break;
            }
        }

        private bool IsLoginFormValid()
        {
            return IsTableTitleValid() && IsSmallBlindValid() && IsMaxPlayersValid();
        }

        private bool IsTableTitleValid()
        {
            if (tableTitleInputField.text == string.Empty)
            {
                DisplayMessage("\"Table title\" field is required.");
                return false;
            }

            return true;
        }

        private bool IsSmallBlindValid()
        {
            if (smallBlindInputField.text == string.Empty)
            {
                DisplayMessage("\"Small blind\" field is required.");
                return false;
            }

            int smallBlind = int.Parse(smallBlindInputField.text);

            if (smallBlind < MinSmallBlind || smallBlind > MaxSmallBlind)
            {
                DisplayMessage("\"Small blind\" field must be between " + MinSmallBlind + " and " + MaxSmallBlind +
                               ".");
                return false;
            }

            return true;
        }

        private bool IsMaxPlayersValid()
        {
            if (maxPlayersInputField.text == string.Empty)
            {
                DisplayMessage("\"Max players\" field is required.");
                return false;
            }

            int maxPlayers = int.Parse(maxPlayersInputField.text);

            if (maxPlayers < MinNumberOfPlayers || maxPlayers > MaxNumberOfPlayers)
            {
                DisplayMessage("\"Max players\" field must be between " + MinNumberOfPlayers + " and " +
                               MaxNumberOfPlayers + ".");
                return false;
            }

            return true;
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
