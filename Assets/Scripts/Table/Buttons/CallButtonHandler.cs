using UnityEngine;

namespace Table.Buttons {
    public class CallButtonHandler : MonoBehaviour {
        public void Call() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Call);
            Session.Writer.WriteLine(Session.Username);
        }
    }
}
