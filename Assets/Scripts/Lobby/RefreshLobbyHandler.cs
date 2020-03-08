using Table;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class RefreshLobbyHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text chipCountText;
        [SerializeField] private TMP_Text winCountText;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject tableButton;

        [SerializeField] private GameObject rewardCanvas;
        [SerializeField] private TMP_Text rewardTitleText;
        [SerializeField] private TMP_Text rewardMessageText;
        [SerializeField] private TMP_Text rewardValueText;
        [SerializeField] private StackDisplayer stackDisplayer;

        void Start()
        {
            CheckForLoginReward();
            DisplayClientData();
            RefreshTableList();
        }

        private void CheckForLoginReward()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.LoginReward);
            int responseCode = Session.Reader.Read();
            if (responseCode == -1) return;

            ServerResponse response = (ServerResponse) responseCode;
            if (response == ServerResponse.LoginRewardActive)
            {
                rewardCanvas.SetActive(true);
                rewardTitleText.text = "Login-reward!";
                rewardMessageText.text = "You have been rewarded with:";
                rewardValueText.text = string.Empty;
                stackDisplayer.UpdateStack(Session.ReadInt());
            }
            else if (response == ServerResponse.LoginRewardNotActive)
            {
                rewardCanvas.SetActive(true);
                rewardTitleText.text = "Come back soon!";
                rewardMessageText.text = "Your next reward will be in:";
                rewardValueText.text = Session.ReadLine();
            }
        }

        private void DisplayClientData()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.ClientData);
            Session.Writer.WriteLine(Session.Username);
            Session.ChipCount = Session.ReadInt();
            Session.WinCount = Session.ReadInt();

            usernameText.text = Session.Username;
            chipCountText.text = "Chips: " + Session.ChipCount;
            winCountText.text = "Wins: " + Session.WinCount;
        }

        public void RefreshTableList()
        {
            HideTableButtons();

            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.TableList);
            int tableCount = Session.ReadInt();

            for (int i = 0; i < tableCount; i++)
            {
                ShowTableButton(
                    i, 
                    Session.ReadLine(), 
                    Session.ReadInt(), 
                    Session.ReadInt(), 
                    Session.ReadInt()
                );
            }
        }

        private void ShowTableButton(int index, string title, int smallBlind, int playerCount, int maxPlayers)
        {
            GameObject button;

            if (index < grid.transform.childCount)
            {
                button = grid.transform.GetChild(index).gameObject;
                button.SetActive(true);
            }
            else
            {
                button = Instantiate(tableButton, grid.transform, true);
            }

            button.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text =
                title + " | Blinds: " + smallBlind + "/" + smallBlind * 2 + " | Players: " + playerCount + "/" + maxPlayers;

            TableData data = button.GetComponent<TableData>();
            data.Title = title;
            data.SmallBlind = smallBlind;
            data.PlayerCount = playerCount;
            data.MaxPlayers = maxPlayers;
        }

        private void HideTableButtons()
        {
            foreach (Transform t in grid.transform)
            {
                t.gameObject.SetActive(false);
            }
        }
    }
}