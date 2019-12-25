using System.Threading;
using UnityEngine;

namespace Lobby {
    public class LogoutHandler : MonoBehaviour {
        public void Logout() {
            new Thread(() => {
                Session.Writer.BaseStream.WriteByte((byte) ClientRequest.Logout);
                Session.Finish();
                
                MainThreadExecutor.Instance.Enqueue(() => GetComponent<SceneLoader>().LoadScene());
            }).Start();
        }
    }
}
