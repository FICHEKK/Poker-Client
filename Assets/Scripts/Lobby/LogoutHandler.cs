using UnityEngine;

namespace Lobby {
    public class LogoutHandler : MonoBehaviour {
        public void Logout() {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.Logout);
            Session.Writer.WriteLine(Session.Username);
            Session.Finish();
            
            GetComponent<SceneLoader>().LoadScene();
        }
    }
}
