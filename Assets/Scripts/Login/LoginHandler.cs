using System;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

namespace Login
{
    public class LoginHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text messageText;

        public void Login()
        {
            if (!IsLoginFormValid()) return;

            try
            {
                Session.Username = usernameInputField.text;
                Session.Client = new TcpClient(addressInputField.text, int.Parse(portInputField.text));
                Session.Writer = new StreamWriter(Session.Client.GetStream()) {AutoFlush = true};
                Session.Reader = new StreamReader(Session.Client.GetStream());

                SendLoginDataToServer();
                ProcessServerResponse(Session.Reader.Read());
            }
            catch (SocketException)
            {
                DisplayMessage("Error establishing the connection with the server.");
            }
            catch (Exception e)
            {
                DisplayMessage(e.Message);
            }
        }

        private void SendLoginDataToServer()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.Login);
            Session.Writer.WriteLine(usernameInputField.text);
            Session.Writer.WriteLine(passwordInputField.text);
        }

        private void ProcessServerResponse(int responseCode)
        {
            if (responseCode == -1)
            {
                Session.Finish();
                DisplayMessage("Server connection error.");
                return;
            }

            ServerLoginResponse response = (ServerLoginResponse) responseCode;

            if (response == ServerLoginResponse.Success)
            {
                Session.HasJustLoggedIn = true;
                GetComponent<SceneLoader>().LoadScene();
                return;
            }

            switch (response)
            {
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
                    DisplayMessage("Account has been banned.");
                    break;
                default:
                    DisplayMessage("Unexpected error occurred. Please try again.");
                    break;
            }
            
            Session.Finish();
        }

        private bool IsLoginFormValid()
        {
            if (usernameInputField.text == string.Empty)
            {
                DisplayMessage("Username is required.");
                return false;
            }

            if (passwordInputField.text == string.Empty)
            {
                DisplayMessage("Password is required.");
                return false;
            }

            if (!SocketAddressValidator.ValidateAddress(addressInputField.text))
            {
                DisplayMessage("Invalid IP address.");
                return false;
            }

            if (!SocketAddressValidator.ValidatePort(portInputField.text))
            {
                DisplayMessage("Invalid port number.");
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
