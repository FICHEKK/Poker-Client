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

        private const int MinimumUsernameLength = 3;
        private const int MinimumPasswordLength = 8;

        public void Login() {
            if (!IsLoginFormValid()) {
                Debug.Log("Registration form is not valid.");
                return;
            }
            
            string address = addressInputField.text;
            int port = int.Parse(portInputField.text);

            using (TcpClient client = new TcpClient(address, port))
            using (StreamReader reader = new StreamReader(client.GetStream()))
            using (StreamWriter writer = new StreamWriter(client.GetStream())) {
                writer.BaseStream.WriteByte((byte) ClientRequest.Login);
                writer.WriteLine(usernameInputField.text);
                writer.WriteLine(passwordInputField.text);
                writer.Flush();

                int responseCode = writer.BaseStream.ReadByte();
                if (responseCode == -1) return;

                ServerResponse response = (ServerResponse) responseCode;

                if (response == ServerResponse.LoginSucceeded) {
                    PlayerPrefs.SetString("username", usernameInputField.text);
                    PlayerPrefs.SetString("chipCount", reader.ReadLine());
                    PlayerPrefs.SetString("winCount", reader.ReadLine());
                    
                    GetComponent<SceneLoader>().LoadScene();
                }
                else {
                    Debug.Log("Login failed: " + response);
                }
            }
        }

        private bool IsLoginFormValid() {
            if (usernameInputField.text.Length < MinimumUsernameLength) return false;
            if (passwordInputField.text.Length < MinimumPasswordLength) return false;
            if (addressInputField.text == string.Empty) return false;
            if (portInputField.text == string.Empty) return false;

            return true;
        }
    }
}
