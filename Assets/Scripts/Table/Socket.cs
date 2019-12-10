using UnityEngine;
using UnityEngine.UI;

namespace Table {
    public class Socket : MonoBehaviour {
        [SerializeField] private Slider _slider;

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
