using TMPro;
using UnityEngine;

namespace Login {
    public class LoginPerformer : MonoBehaviour {
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private TMP_InputField portInputField;

        private const int MinimumUsernameLength = 3;
        private const int MinimumPasswordLength = 8;

        public void Login() {
            if (!IsLoginFormValid()) {
                Debug.Log("Form is not valid.");
                return;
            }

            Debug.Log("Connecting to " + addressInputField.text + ":" + portInputField.text);
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
