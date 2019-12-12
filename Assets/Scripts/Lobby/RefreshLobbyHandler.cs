using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class RefreshLobbyHandler : MonoBehaviour {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text chipCountText;
        [SerializeField] private TMP_Text winCountText;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject tableButton;

        void Start() {
            usernameText.text = Session.Username;
            
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.ClientData);
            Session.Writer.WriteLine(Session.Username);
            Session.Writer.Flush();

            Session.ChipCount = int.Parse(Session.Reader.ReadLine());
            Session.WinCount = int.Parse(Session.Reader.ReadLine());

            chipCountText.text = "Chips: " + Session.ChipCount;
            winCountText.text = "Wins: " + Session.WinCount;
            
            RefreshLobby();
        }

        public void RefreshLobby() {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.TableList);

            int tableCount = int.Parse(Session.Reader.ReadLine());

            foreach (Transform t in grid.transform) {
                t.gameObject.SetActive(false);
            }

            for (int i = 0; i < tableCount; i++) {
                string title = Session.Reader.ReadLine();
                int smallBlind = int.Parse(Session.Reader.ReadLine());
                int bigBlind = smallBlind * 2;
                int playerCount = int.Parse(Session.Reader.ReadLine());
                int maxPlayers = int.Parse(Session.Reader.ReadLine());

                GameObject button;
                
                if (i < grid.transform.childCount) {
                    button = grid.transform.GetChild(i).gameObject;
                    button.SetActive(true);
                }
                else {
                    button = Instantiate(tableButton, grid.transform, true);
                }
                
                button.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text =
                    title + " | Blinds: " + smallBlind + "/" + bigBlind + " | Players: " + playerCount + "/" + maxPlayers;
                
                TableData data = button.GetComponent<TableData>();
                data.Title = title;
                data.SmallBlind = smallBlind;
                data.PlayerCount = playerCount;
                data.MaxPlayers = maxPlayers;
            }
        }
    }
}
