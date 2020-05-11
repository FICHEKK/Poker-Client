using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class TableButtonHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text joinTableTitleText;
        [SerializeField] private Slider joinTableSlider;
        [SerializeField] private TMP_Text joinTableBuyInText;
        [SerializeField] private GameObject joinTableButton;

        private const int MinSmallBlindsToJoin = 20;

        public void ShowJoinTableWindow()
        {
            HideMessage();

            TableData data = GetComponent<TableData>();

            if (Session.ChipCount < data.SmallBlind * MinSmallBlindsToJoin)
            {
                DisplayMessage("You do not have enough chips to join this table.");
                return;
            }

            if (data.PlayerCount == data.MaxPlayers)
            {
                DisplayMessage("Table is full.");
                return;
            }

            GetComponent<GameObjectToggle>().ShowGameObject();

            var mode = data.IsRanked ? "Ranked" : "Standard";
            joinTableTitleText.text =
                $"{data.Title}{Environment.NewLine}" +
                $"Blinds: {data.SmallBlind}/{data.SmallBlind * 2} | " +
                $"Players: {data.PlayerCount}/{data.MaxPlayers} | {mode}";

            joinTableSlider.minValue = data.SmallBlind * 2 * 10;
            joinTableSlider.maxValue = Math.Min(Session.ChipCount, data.SmallBlind * 2 * 200);
            joinTableSlider.value = joinTableSlider.minValue;

            joinTableBuyInText.text = "Buy-In: " + data.SmallBlind * MinSmallBlindsToJoin;
            joinTableButton.GetComponent<TableData>().Overwrite(data);
        }

        //----------------------------------------------------------------
        //                      Message display
        //----------------------------------------------------------------

        private void DisplayMessage(string text)
        {
            messageText.enabled = true;
            messageText.text = text;
        }

        private void HideMessage()
        {
            messageText.enabled = false;
        }
    }
}
