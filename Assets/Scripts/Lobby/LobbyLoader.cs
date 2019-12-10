using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class LobbyLoader : MonoBehaviour {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text chipCountText;
        [SerializeField] private TMP_Text winCountText;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject tableButton;

        void Start() {
            DisplayUserInformation();
            RefreshTableList();
        }
        
        private void DisplayUserInformation() {
            usernameText.text = Session.Username;
            chipCountText.text =  "Chips: " + Session.ChipCount;
            winCountText.text = "Wins: " + Session.WinCount;
        }

        public void RefreshTableList() {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.TableList);

            int tableCount = int.Parse(Session.Reader.ReadLine());

            foreach (Transform t in grid.transform) {
                Destroy(t.gameObject);
            }

            for (int i = 0; i < tableCount; i++) {
                string title = Session.Reader.ReadLine();
                int smallBlind = int.Parse(Session.Reader.ReadLine());
                int bigBlind = smallBlind * 2;
                int playerCount = int.Parse(Session.Reader.ReadLine());
                int maxPlayers = int.Parse(Session.Reader.ReadLine());

                string buttonTitle = title + " | Blinds: " + smallBlind + "/" + bigBlind + " | Players: " + playerCount + "/" + maxPlayers;
                GameObject button = Instantiate(tableButton, grid.transform, true);
                button.transform.localScale = new Vector3(1, 1, 1);
                button.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = buttonTitle;

                TableData data = button.GetComponent<TableData>();
                data.Title = title;
                data.SmallBlind = smallBlind;
                data.PlayerCount = playerCount;
                data.MaxPlayers = maxPlayers;
            }
        }
    }
}
