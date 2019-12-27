using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class RefreshLobbyHandler : MonoBehaviour {
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text chipCountText;
        [SerializeField] private TMP_Text winCountText;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject tableButton;

        void Start() {
            new Thread(() => {
                Session.Writer.BaseStream.WriteByte((byte) ClientRequest.ClientData);
                Session.Writer.WriteLine(Session.Username);
                Session.ChipCount = ReadInt();
                Session.WinCount = ReadInt();
                
                MainThreadExecutor.Instance.Enqueue(() => {
                    usernameText.text = Session.Username;
                    chipCountText.text = "Chips: " + Session.ChipCount;
                    winCountText.text = "Wins: " + Session.WinCount;
            
                    RefreshTableList();
                });
            }).Start();
        }

        public void RefreshTableList() {
            HideTableButtons();
            refreshButton.interactable = false;

            new Thread(() => {
                Session.Writer.BaseStream.WriteByte((byte) ClientRequest.TableList);
                int tableCount = ReadInt();

                for (int i = 0; i < tableCount; i++) {
                    ShowTableButton(i, ReadLine(), ReadInt(), ReadInt(), ReadInt());
                }
                
                MainThreadExecutor.Instance.Enqueue(() => refreshButton.interactable = true);
            }).Start();
        }

        private void ShowTableButton(int index, string title, int smallBlind, int playerCount, int maxPlayers) {
            MainThreadExecutor.Instance.Enqueue(() => {
                GameObject button;
                
                if (index < grid.transform.childCount) {
                    button = grid.transform.GetChild(index).gameObject;
                    button.SetActive(true);
                }
                else {
                    button = Instantiate(tableButton, grid.transform, true);
                }
                
                button.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text =
                    title + " | Blinds: " + smallBlind + "/" + smallBlind * 2 + " | Players: " + playerCount + "/" + maxPlayers;
                
                TableData data = button.GetComponent<TableData>();
                data.Title = title;
                data.SmallBlind = smallBlind;
                data.PlayerCount = playerCount;
                data.MaxPlayers = maxPlayers;
            });
        }
        
        private void HideTableButtons() {
            foreach (Transform t in grid.transform) {
                t.gameObject.SetActive(false);
            }
        }

        private int ReadInt() => int.Parse(ReadLine());
        private string ReadLine() => Session.Reader.ReadLine();
    }
}
