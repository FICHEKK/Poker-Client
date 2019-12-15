using UnityEngine;

namespace Table.Buttons {
    public class FoldButtonHandler : MonoBehaviour {
        public void Fold() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Fold);
            Session.Writer.WriteLine(Session.Username);
        }
    }
}
