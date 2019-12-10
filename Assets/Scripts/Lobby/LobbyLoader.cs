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
                string tableName = Session.Reader.ReadLine();
                string smallBlind = Session.Reader.ReadLine();
                string bigBlind = (int.Parse(smallBlind) * 2).ToString();
                string playerCount = Session.Reader.ReadLine();
                string maxPlayers = Session.Reader.ReadLine();

                string buttonTitle = tableName + " | Blinds: " + smallBlind + "/" + bigBlind +
                                                 " | Players: " + playerCount + "/" + maxPlayers;
                GameObject button = Instantiate(tableButton, grid.transform, true);
                button.transform.localScale = new Vector3(1, 1, 1);
                button.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = buttonTitle;
            }
        }
    }
}
