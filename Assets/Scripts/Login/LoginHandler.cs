using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

namespace Login {
    public class LoginHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text messageText;

        public void Login() {
            if (!IsLoginFormValid()) return;

            string address = addressInputField.text;
            int port = int.Parse(portInputField.text);

            try {
                TcpClient client = new TcpClient(address, port);
                StreamWriter writer = new StreamWriter(client.GetStream());

                writer.BaseStream.WriteByte((byte) ClientRequest.Login);
                writer.WriteLine(usernameInputField.text);
                writer.WriteLine(passwordInputField.text);
                writer.Flush();

                int responseCode = writer.BaseStream.ReadByte();
                if (responseCode == -1) return;

                ServerResponse response = (ServerResponse) responseCode;

                if (response == ServerResponse.LoginSucceeded) {
                    StreamReader reader = new StreamReader(client.GetStream());
                    Session.Username = usernameInputField.text;
                    Session.ChipCount = reader.ReadLine();
                    Session.WinCount = reader.ReadLine();
                    
                    Session.Client = client;
                    Session.Reader = reader;
                    Session.Writer = writer;

                    Trace.WriteLine("Učitavam...");
                    GetComponent<SceneLoader>().LoadScene();
                }
                else {
                    writer.Close();
                    client.Close();
                    NotifyPlayer(response);
                }
            }
            catch (SocketException) {
                DisplayMessage("Error establishing the connection with the server.");
            }
        }

        private void NotifyPlayer(ServerResponse response) {
            switch (response) {
                case ServerResponse.LoginFailedServerIsFull:
                    DisplayMessage("Server is full.");
                    break;
                case ServerResponse.LoginFailedUsernameNotRegistered:
                case ServerResponse.LoginFailedWrongPassword:
                    DisplayMessage("Invalid username or password.");
                    break;
                case ServerResponse.LoginFailedClientAlreadyLoggedIn:
                    DisplayMessage("You are already logged in.");
                    break;
                case ServerResponse.LoginFailedClientIsBanned:
                    DisplayMessage("You have been banned.");
                    break;
                default:
                    DisplayMessage("Unexpected error occurred. Please try again.");
                    break;
            }
        }

        private bool IsLoginFormValid() {
            HideMessage();

            if (usernameInputField.text == string.Empty) {
                DisplayMessage("Username is required.");
                return false;
            }

            if (passwordInputField.text == string.Empty) {
                DisplayMessage("Password is required.");
                return false;
            }
            
            if (!SocketAddressValidator.ValidateAddress(addressInputField.text)) {
                DisplayMessage("Invalid IP address.");
                return false;
            }
            
            if (!SocketAddressValidator.ValidatePort(portInputField.text)) {
                DisplayMessage("Invalid port number.");
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
