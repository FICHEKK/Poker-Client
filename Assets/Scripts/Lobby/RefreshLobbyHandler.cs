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
        [SerializeField] private TMP_Text eloRatingText;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject tableButton;

        // Reward canvas
        [SerializeField] private GameObject rewardCanvas;
        [SerializeField] private TMP_Text rewardTitleText;
        [SerializeField] private TMP_Text rewardMessageText;
        [SerializeField] private TMP_Text rewardValueText;
        [SerializeField] private StackDisplayer stackDisplayer;
        
        // Leave table message canvas
        [SerializeField] private GameObject leaveTableCanvas;
        [SerializeField] private TMP_Text leaveTableTitleText;
        [SerializeField] private TMP_Text leaveTableMessageText;

        private static readonly Color OddButtonColor = new Color(.9f, .9f, .9f);

        private void Start()
        {
            CheckForLeaveTableReason();
            CheckForLoginReward();
            DisplayClientData();
            RefreshTableList();
        }

        private void CheckForLeaveTableReason()
        {
            if (Session.LeaveTableReason == null) return;

            switch (Session.LeaveTableReason)
            {
                case ServerResponse.LeaveTableGranted:
                    Debug.Log("You left the table willingly.");
                    break;
                
                case ServerResponse.LeaveTableNoMoney:
                    leaveTableCanvas.SetActive(true);
                    leaveTableTitleText.text = "Bankrupt";
                    leaveTableMessageText.text = "Unfortunately, you went bankrupt.";
                    break;
                
                case ServerResponse.LeaveTableRanked:
                    var placeFinished = Session.ReadInt();
                    var oldRating = Session.ReadInt();
                    var newRating = Session.ReadInt();
                    
                    leaveTableCanvas.SetActive(true);
                    leaveTableTitleText.text = "Ranked match result";
                    leaveTableMessageText.text = "Place finished: " + placeFinished + "\n" +
                                                 "Old rating: " + oldRating + "\n" +
                                                 "New rating: " + newRating;
                    break;
            }

            Session.LeaveTableReason = null;
        }

        private void CheckForLoginReward()
        {
            if (!Session.HasJustLoggedIn) return;
            
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

            Session.HasJustLoggedIn = false;
        }

        private void DisplayClientData()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.ClientData);
            Session.Writer.WriteLine(Session.Username);
            Session.ChipCount = Session.ReadInt();
            Session.WinCount = Session.ReadInt();
            Session.EloRating = Session.ReadInt();

            usernameText.text = Session.Username;
            chipCountText.text = "Chips: " + Session.ChipCount;
            winCountText.text = "Wins: " + Session.WinCount;
            eloRatingText.text = "Rating: " + Session.EloRating;
        }

        public void RefreshTableList()
        {
            HideTableButtons();

            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.TableList);
            var tableCount = Session.ReadInt();

            for (int i = 0; i < tableCount; i++)
            {
                ShowTableButton(
                    i, 
                    Session.ReadLine(), 
                    Session.ReadInt(), 
                    Session.ReadInt(), 
                    Session.ReadInt(),
                    Session.ReadBool(),
                    Session.ReadBool()
                );
            }
        }

        private void ShowTableButton(int index, string title, int smallBlind, int playerCount, int maxPlayers, bool isRanked, bool isLocked)
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

            if (index % 2 == 1) button.GetComponent<Image>().color = OddButtonColor;

            var mode = isRanked ? "Ranked" : "Standard";
            var locked = isLocked ? "Locked" : "";
            button.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text =
                $"{title} | Blinds: {smallBlind}/{smallBlind * 2} | Players: {playerCount}/{maxPlayers} | {mode} {locked}";

            var data = button.GetComponent<TableData>();
            data.Title = title;
            data.SmallBlind = smallBlind;
            data.PlayerCount = playerCount;
            data.MaxPlayers = maxPlayers;
            data.IsRanked = isRanked;
            data.IsLocked = isLocked;
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