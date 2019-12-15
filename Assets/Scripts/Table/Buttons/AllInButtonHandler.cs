using UnityEngine;

namespace Table.Buttons {
    public class AllInButtonHandler : MonoBehaviour {
        public void AllIn() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.AllIn);
            Session.Writer.WriteLine(Session.Username);
        }
    }
}
