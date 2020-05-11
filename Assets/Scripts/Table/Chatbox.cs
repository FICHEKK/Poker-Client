using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table
{
    public class Chatbox : MonoBehaviour
    {
        [SerializeField] private TMP_InputField chatInputField;
        [SerializeField] private Button chatSendButton;

        private void Awake()
        {
            chatInputField.onValueChanged.AddListener(text => chatSendButton.interactable = !string.IsNullOrEmpty(text));
            chatSendButton.onClick.AddListener(SendChatMessage);
        }
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && chatInputField.interactable)
            {
                SendChatMessage();
                chatInputField.Select();
            }
        }

        private void SendChatMessage()
        {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.SendChatMessage);
            Session.Writer.WriteLine(chatInputField.text.Replace("\n", "").Replace("\r", ""));
            chatInputField.text = string.Empty;
        }
    }
}
