using TMPro;
using UnityEngine;

namespace Lobby {
    public class InformationFiller : MonoBehaviour {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text chipCountText;
        [SerializeField] private TMP_Text winCountText;
    
        void Start() {
            usernameText.text = PlayerPrefs.GetString("username");
            chipCountText.text = PlayerPrefs.GetString("chipCount");
            winCountText.text = PlayerPrefs.GetString("winCount");
        }
    }
}
