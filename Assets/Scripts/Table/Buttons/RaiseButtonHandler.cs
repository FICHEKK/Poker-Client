using UnityEngine;
using UnityEngine.UI;

namespace Table.Buttons {
    public class RaiseButtonHandler : MonoBehaviour {
        [SerializeField] private Slider slider;
        
        public void Raise() {
            Session.Client.GetStream().WriteByte((byte) ClientRequest.Raise);
            Session.Writer.WriteLine(Session.Username);
            Session.Writer.WriteLine(slider.value);
        }
    }
}
