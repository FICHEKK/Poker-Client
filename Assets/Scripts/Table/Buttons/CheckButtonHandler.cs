using UnityEngine;

namespace Table.Buttons {
    public class CheckButtonHandler : MonoBehaviour {
        public void Check() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Check);
            Session.Writer.WriteLine(Session.Username);
        }
    }
}
