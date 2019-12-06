﻿using System.IO;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

namespace Register {
    public class RegistrationHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField confirmPasswordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text messageText;

        private static readonly Color ServerSuccessColor = new Color(0f, 0.5f, 0f);
        private static readonly Color ServerErrorColor = new Color(1f, 0.5f, 0f);
        private static readonly Color FormErrorColor = Color.red;
        
        private const int MinimumUsernameLength = 2;
        private const int MaximumUsernameLength = 32;
        
        private const int MinimumPasswordLength = 8;
        private const int MaximumPasswordLength = 64;

        public void Register() {
            if (!IsRegistrationFormValid()) return;

            string address = addressInputField.text;
            int port = int.Parse(portInputField.text);

            try {
                using (TcpClient client = new TcpClient(address, port))
                using (StreamWriter writer = new StreamWriter(client.GetStream())) {
                    writer.BaseStream.WriteByte((byte) ClientRequest.Register);
                    writer.WriteLine(usernameInputField.text);
                    writer.WriteLine(passwordInputField.text);
                    writer.Flush();

                    int responseCode = writer.BaseStream.ReadByte();
                    if (responseCode != -1) {
                        NotifyPlayer((ServerResponse) responseCode);
                    }
                    else {
                        Debug.Log("Connection with the server was closed.");
                    }
                }
            }
            catch (SocketException) {
                DisplayMessage("Error establishing the connection with the server.", ServerErrorColor);
            }
        }

        private void NotifyPlayer(ServerResponse response) {
            switch (response) {
                case ServerResponse.RegistrationSucceeded:
                    DisplayMessage("Successfully registered!", ServerSuccessColor);
                    break;
                case ServerResponse.RegistrationFailedUsernameAlreadyTaken:
                    DisplayMessage("Username is already taken.", ServerErrorColor);
                    break;
                case ServerResponse.RegistrationFailedIOError:
                    DisplayMessage("Server error occurred. Please try again.", ServerErrorColor);
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
            string address = addressInputField.text;
            
            try {
                IPAddress.Parse(address);
                return true;
            }
            catch {
                DisplayMessage("Invalid IP address.", FormErrorColor);
                return false;
            }
        }
        
        //----------------------------------------------------------------
        //                      Port validation
        //----------------------------------------------------------------
        
        private bool IsPortValid() {
            string port = portInputField.text;
            
            try {
                short.Parse(port);
                return true;
            }
            catch {
                DisplayMessage("Invalid port number.", FormErrorColor);
                return false;
            }
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