using TMPro;
using UnityEngine;

namespace Lobby {
    public class CreateTableHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField tableTitleInputField;
        [SerializeField] private TMP_InputField smallBlindInputField;
        [SerializeField] private TMP_InputField maxPlayersInputField;
        [SerializeField] private TMP_Text messageText;

        private const int MinSmallBlind = 1;
        private const int MaxSmallBlind = 1_000_000;
    
        private const int MinNumberOfPlayers = 2;
        private const int MaxNumberOfPlayers = 9;
    
        public void CreateTable() {
            if (!IsLoginFormValid()) return;
        
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.CreateTable);
            Session.Writer.WriteLine(tableTitleInputField.text);
            Session.Writer.WriteLine(smallBlindInputField.text);
            Session.Writer.WriteLine(maxPlayersInputField.text);
            Session.Writer.Flush();

            int responseCode = Session.Reader.BaseStream.ReadByte();
            if (responseCode == -1) {
                DisplayMessage("Server connection error.");
                return;
            }

            ServerResponse response = (ServerResponse) responseCode;
            if (response == ServerResponse.TableCreationSucceeded) {
                DisplayMessage("Table \"" + tableTitleInputField.text + "\" was created!");
            }
            else if(response == ServerResponse.TableCreationFailedTitleAlreadyTaken) {
                DisplayMessage("Table with title \"" + tableTitleInputField.text + "\" already exists. Table was not created.");
            }
            else {
                DisplayMessage("Invalid server response. Please try again.");
            }
        }

        private bool IsLoginFormValid() {
            HideMessage();
            return IsTableTitleValid() && IsSmallBlindValid() && IsMaxPlayersValid();
        }

        private bool IsTableTitleValid() {
            if (tableTitleInputField.text == string.Empty) {
                DisplayMessage("\"Table title\" field is required.");
                return false;
            }

            return true;
        }
    
        private bool IsSmallBlindValid() {
            if (smallBlindInputField.text == string.Empty) {
                DisplayMessage("\"Small blind\" field is required.");
                return false;
            }
            
            int smallBlind = int.Parse(smallBlindInputField.text);

            if (smallBlind < MinSmallBlind || smallBlind > MaxSmallBlind) {
                DisplayMessage("\"Small blind\" field must be between " + MinSmallBlind + " and " + MaxSmallBlind + ".");
                return false;
            }

            return true;
        }
    
        private bool IsMaxPlayersValid() {
            if (maxPlayersInputField.text == string.Empty) {
                DisplayMessage("\"Max players\" field is required.");
                return false;
            }
            
            int maxPlayers = int.Parse(maxPlayersInputField.text);

            if (maxPlayers < MinNumberOfPlayers || maxPlayers > MaxNumberOfPlayers) {
                DisplayMessage("\"Max players\" field must be between " + MinNumberOfPlayers + " and " + MaxNumberOfPlayers + ".");
                return false;
            }

            return true;
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
