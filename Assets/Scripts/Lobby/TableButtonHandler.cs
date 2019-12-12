using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class TableButtonHandler : MonoBehaviour {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text joinTableTitleText;
        [SerializeField] private Slider joinTableSlider;
        [SerializeField] private TMP_Text joinTableBuyInText;
        [SerializeField] private GameObject joinTableButton;

        private const int MinSmallBlindsToJoin = 20;

        public void ShowJoinTableWindow() {
            HideMessage();

            TableData data = GetComponent<TableData>();

            if (Session.ChipCount < data.SmallBlind * MinSmallBlindsToJoin) {
                DisplayMessage("You do not have enough chips to join this table.");
                return;
            }

            GetComponent<GameObjectToggle>().ShowGameObject();
            joinTableTitleText.text = data.Title + Environment.NewLine
                                                 + "Blinds: " + data.SmallBlind + "/" + data.SmallBlind * 2 + " | "
                                                 + "Players: " + data.PlayerCount + "/" + data.MaxPlayers;

            joinTableSlider.value = 0f;
            joinTableBuyInText.text = "Buy-In: " + data.SmallBlind * MinSmallBlindsToJoin;
            joinTableButton.GetComponent<TableData>().Overwrite(data);
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
