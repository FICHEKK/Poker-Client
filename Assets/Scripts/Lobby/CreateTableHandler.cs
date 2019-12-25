using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class CreateTableHandler : MonoBehaviour {
        [SerializeField] private Button createTableButton;
        [SerializeField] private Button cancelButton;
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

            createTableButton.interactable = false;
            cancelButton.interactable = false;
            DisplayMessage("Connecting to the server...");

            string tableTitle = tableTitleInputField.text;
            string smallBlind = smallBlindInputField.text;
            string maxPlayers = maxPlayersInputField.text;
            
            new Thread(() => {
                Session.Writer.BaseStream.WriteByte((byte) ClientRequest.CreateTable);
                Session.Writer.WriteLine(tableTitle);
                Session.Writer.WriteLine(smallBlind);
                Session.Writer.WriteLine(maxPlayers);
                
                int responseCode = Session.Reader.Read();
                if (responseCode == -1) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Server connection error."));
                    return;
                }
                
                ServerCreateTableResponse response = (ServerCreateTableResponse) responseCode;

                if (response == ServerCreateTableResponse.Success) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Table \"" + tableTitle + "\" was created!"));
                }
                else if(response == ServerCreateTableResponse.TitleTaken) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Table \"" + tableTitle + "\" already exists."));
                }
                else {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Invalid server response. Please try again."));
                }
                
                MainThreadExecutor.Instance.Enqueue(() => {
                    createTableButton.interactable = true;
                    cancelButton.interactable = true;
                });
            }).Start();
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
