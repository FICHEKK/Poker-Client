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
        private const int MaxSmallBlindsToJoin = 200;

        public void ShowJoinTableWindow()
        {
            var table = GetComponent<TableData>();
            if (!JoinConditionsSatisfied(table)) return;
            
            GetComponent<GameObjectToggle>().ShowGameObject();

            var mode = table.IsRanked ? "Ranked" : "Standard";
            joinTableTitleText.text =
                $"{table.Title}{Environment.NewLine}" +
                $"Blinds: {table.SmallBlind}/{table.SmallBlind * 2} | " +
                $"Players: {table.PlayerCount}/{table.MaxPlayers} | {mode}";

            joinTableSlider.minValue = table.SmallBlind * MinSmallBlindsToJoin;
            joinTableSlider.maxValue = Math.Min(Session.ChipCount, table.SmallBlind * MaxSmallBlindsToJoin);
            
            if (table.IsRanked)
            {
                joinTableSlider.value = joinTableSlider.maxValue;
                joinTableSlider.interactable = false;
            }
            else
            {
                joinTableSlider.value = joinTableSlider.minValue;
                joinTableSlider.interactable = true;
            }

            joinTableBuyInText.text = "Buy-In: " + joinTableSlider.value;
            joinTableButton.GetComponent<TableData>().Overwrite(table);
        }

        private bool JoinConditionsSatisfied(TableData table)
        {
            var message = string.Empty;
            
            if (table.IsLocked)
            {
                message = "Cannot join: Table is locked.";
            }
            else if (table.PlayerCount == table.MaxPlayers)
            {
                message = "Cannot join: Table is full.";
            }
            else
            {
                var requiredChipCount = table.SmallBlind * (table.IsRanked ? MaxSmallBlindsToJoin : MinSmallBlindsToJoin);
                var difference = Session.ChipCount - requiredChipCount;

                if (difference < 0)
                {
                    message = $"Cannot join: Not enough chips (missing {Math.Abs(difference)} chips).";
                }
            }
            
            DisplayMessage(message);
            return message == string.Empty;
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
