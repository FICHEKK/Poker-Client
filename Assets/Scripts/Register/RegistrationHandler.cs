using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Table;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Register {
    public class RegistrationHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField confirmPasswordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button registerButton;

        private static readonly Color InfoMessageColor = Color.white;
        private static readonly Color ServerSuccessColor = new Color(0f, 0.5f, 0f);
        private static readonly Color ServerErrorColor = new Color(1f, 0.5f, 0f);
        private static readonly Color FormErrorColor = Color.red;
        
        private const int MinimumUsernameLength = 2;
        private const int MaximumUsernameLength = 32;
        
        private const int MinimumPasswordLength = 8;
        private const int MaximumPasswordLength = 64;

        public void Register() {
            if (!IsRegistrationFormValid()) return;

            registerButton.interactable = false;
            DisplayMessage("Connecting to the server...", InfoMessageColor);

            new Thread(() => {
                try {
                    RegisterOnServer(addressInputField.text, int.Parse(portInputField.text));
                }
                catch (SocketException) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage("Error establishing the connection with the server.", ServerErrorColor));
                }
                catch (Exception e) {
                    MainThreadExecutor.Instance.Enqueue(() => DisplayMessage(e.Message, ServerErrorColor));
                }
                
                MainThreadExecutor.Instance.Enqueue(() => registerButton.interactable = true);
            }).Start();
        }

        private void RegisterOnServer(string serverAddress, int serverPort) {
            using (TcpClient client = new TcpClient(serverAddress, serverPort))
            using (StreamWriter writer = new StreamWriter(client.GetStream()) {AutoFlush = true}) {
                writer.BaseStream.WriteByte((byte) ClientRequest.Register);
                writer.WriteLine(usernameInputField.text);
                writer.WriteLine(passwordInputField.text);

                int responseCode = client.GetStream().ReadByte();
                if (responseCode == -1) return;

                MainThreadExecutor.Instance.Enqueue(() => NotifyPlayer((ServerRegistrationResponse) responseCode));
            }
        }

        private void NotifyPlayer(ServerRegistrationResponse response) {
            switch (response) {
                case ServerRegistrationResponse.Success:
                    DisplayMessage("Successfully registered!", ServerSuccessColor);
                    break;
                case ServerRegistrationResponse.UsernameTaken:
                    DisplayMessage("Username is already taken.", ServerErrorColor);
                    break;
                case ServerRegistrationResponse.DatabaseError:
                    DisplayMessage("Server database error occurred. Please try again.", ServerErrorColor);
                    break;
                default:
                    DisplayMessage("Unexpected error occurred. Please try again.", ServerErrorColor);
                    break;
            }
        }
        
        //----------------------------------------------------------------
        //                      Form validation
        //----------------------------------------------------------------
        
        private bool IsRegistrationFormValid() {
            HideMessage();
            return IsUsernameValid() && IsPasswordValid() && IsAddressValid() && IsPortValid();
        }

        //----------------------------------------------------------------
        //                      Username validation
        //----------------------------------------------------------------
        
        private bool IsUsernameValid() {
            string username = usernameInputField.text;
            string errorMessage = null;

            if (username.Length < MinimumUsernameLength) {
                errorMessage = "Username should be at least " + MinimumUsernameLength + " symbols long.";
            }
            else if (username.Length > MaximumUsernameLength) {
                errorMessage = "Username cannot be longer than " + MaximumUsernameLength + " symbols.";
            }
            else if (!IsUsernameCorrectlyDefined(username)) {
                errorMessage = "Username can only contain letters and digits.";
            }

            if (errorMessage != null) {
                DisplayMessage(errorMessage, FormErrorColor);
                return false;
            }

            return true;
        }

        private bool IsUsernameCorrectlyDefined(string username) {
            foreach (char c in username) {
                if (!IsValidUsernameCharacter(c)) {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidUsernameCharacter(char character) {
            return char.IsLetter(character) || char.IsDigit(character);
        }

        //----------------------------------------------------------------
        //                      Password validation
        //----------------------------------------------------------------

        private bool IsPasswordValid() {
            string password = passwordInputField.text;
            string confirmedPassword = confirmPasswordInputField.text;
            string errorMessage = null;

            if (password.Length < MinimumPasswordLength) {
                errorMessage = "Password should be at least " + MinimumPasswordLength + " symbols long.";
            }
            else if (password.Length > MaximumPasswordLength) {
                errorMessage = "Password cannot be longer than " + MaximumPasswordLength + " symbols.";
            }
            else if (password != confirmedPassword) {
                errorMessage = "Password and confirmed password do not match.";
            }

            if (errorMessage != null) {
                DisplayMessage(errorMessage, FormErrorColor);
                return false;
            }

            return true;
        }
        
        //----------------------------------------------------------------
        //                      Address validation
        //----------------------------------------------------------------
        
        private bool IsAddressValid() {
            bool isValid = SocketAddressValidator.ValidateAddress(addressInputField.text);

            if (!isValid) {
                DisplayMessage("Invalid IP address.", FormErrorColor);
            }
            
            return isValid;
        }
        
        //----------------------------------------------------------------
        //                      Port validation
        //----------------------------------------------------------------
        
        private bool IsPortValid() {
            bool isValid = SocketAddressValidator.ValidatePort(portInputField.text);
            
            if (!isValid) {
                DisplayMessage("Invalid port number.", FormErrorColor);
            }

            return isValid;
        }
        
        //----------------------------------------------------------------
        //                      Message display
        //----------------------------------------------------------------

        private void DisplayMessage(string text, Color color) {
            messageText.enabled = true;
            messageText.text = text;
            messageText.color = color;
        }

        private void HideMessage() {
            messageText.enabled = false;
        }
    }
}