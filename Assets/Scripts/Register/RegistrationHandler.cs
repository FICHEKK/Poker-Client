using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

namespace Register {
    public class RegistrationHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;

        private const int MinimumUsernameLength = 3;
        private const int MinimumPasswordLength = 8;

        public void Register() {
            if (!IsRegistrationFormValid()) {
                Debug.Log("Registration form is not valid.");
                return;
            }
            
            string address = addressInputField.text;
            int port = int.Parse(portInputField.text);

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

        private void NotifyPlayer(ServerResponse response) {
            switch (response) {
                case ServerResponse.RegistrationSucceeded:
                    Debug.Log("Success!");
                    break;
                case ServerResponse.RegistrationFailedIOError:
                    Debug.Log("IO Error");
                    break;
                case ServerResponse.RegistrationFailedUsernameAlreadyTaken:
                    Debug.Log("Username taken");
                    break;
                default:
                    Debug.Log("Invalid response '" + response + "'");
                    break;
            }
        }

        private bool IsRegistrationFormValid() {
            if (usernameInputField.text.Length < MinimumUsernameLength) return false;
            if (passwordInputField.text.Length < MinimumPasswordLength) return false;
            if (addressInputField.text == string.Empty) return false;
            if (portInputField.text == string.Empty) return false;

            return true;
        }
    }
}
