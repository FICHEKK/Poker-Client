using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Login {
    
    public class LoginHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button loginButton;

        public void Login() {
            if (!IsLoginFormValid()) return;

            loginButton.interactable = false;
            DisplayMessage("Connecting to the server...");

            new Thread(() => {
                try {
                    TcpClient client = new TcpClient(addressInputField.text, int.Parse(portInputField.text));
                    StreamWriter writer = new StreamWriter(client.GetStream()) {AutoFlush = true};

                    writer.BaseStream.WriteByte((byte) ClientRequest.Login);
                    writer.WriteLine(usernameInputField.text);
                    writer.WriteLine(passwordInputField.text);

                    int responseCode = client.GetStream().ReadByte();
                    if (responseCode == -1) return;

                    ServerLoginResponse response = (ServerLoginResponse) responseCode;

                    if (response == ServerLoginResponse.Success) {
                        Session.Username = usernameInputField.text;
                        Session.Client = client;
                        Session.Reader = new StreamReader(client.GetStream());
                        Session.Writer = writer;

                        MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
                    }
                    else {
                        writer.Close();
                        client.Close();
                        MainThreadExecutor.Instance.Enqueue(() => NotifyPlayer(response));
                    }
                }
                catch (SocketException) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Error establishing the connection with the server."));
                }
                catch (Exception e) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage(e.Message));
                }
                
                MainThreadExecutor.Instance.Enqueue(() => loginButton.interactable = true);
            }).Start();
        }

        private void NotifyPlayer(ServerLoginResponse response) {
            switch (response) {
                case ServerLoginResponse.ServerFull:
                    DisplayMessage("Server is full.");
                    break;
                case ServerLoginResponse.UsernameNotRegistered:
                case ServerLoginResponse.WrongPassword:
                    DisplayMessage("Invalid username or password.");
                    break;
                case ServerLoginResponse.AlreadyLoggedIn:
                    DisplayMessage("You are already logged in.");
                    break;
                case ServerLoginResponse.UsernameBanned:
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
