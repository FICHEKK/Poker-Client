using UnityEngine;

namespace Lobby
{
    public class LogoutHandler : MonoBehaviour
    {
        public void Logout()
        {
            Session.Writer.BaseStream.WriteByte((byte) ClientRequest.Logout);
            Session.Finish();

            GetComponent<SceneLoader>().LoadScene();
        }
    }
}
