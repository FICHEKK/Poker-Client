using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table {
    public class Socket : MonoBehaviour {
        [SerializeField] private TMP_Text waitingForPlayerText;
        [SerializeField] private TMP_Text opponentUsernameText;
        [SerializeField] private TMP_Text opponentChipCountText;
        
        [SerializeField] private Slider _slider;

        private int seatIndex;
        
        public void Start() {
            seatIndex = int.Parse(Session.Reader.ReadLine());
            
            int responseCode = Session.Reader.BaseStream.ReadByte();
            ServerResponse response = (ServerResponse) responseCode;

            if (response == ServerResponse.TableIsEmpty) {
                waitingForPlayerText.enabled = true;

                responseCode = Session.Reader.BaseStream.ReadByte();
                response = (ServerResponse) responseCode;

                if (response == ServerResponse.PlayerJoined) {
                    int joiningPlayerIndex = int.Parse(Session.Reader.ReadLine());
                    string username = Session.Reader.ReadLine();
                    int chipCount = int.Parse(Session.Reader.ReadLine());
                }
            }
        }

        public void Check() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Check);
        }
    
        public void Call() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Call);
        }
    
        public void Fold() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Fold);
        }
    
        public void Raise() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Raise);
        }
    
        public void AllIn() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.AllIn);
        }
    }
}
