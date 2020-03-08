using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class SliderHandler : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void UpdateBuyInText()
        {
            GetComponent<TMP_Text>().text = "Buy-In: " + slider.value;
        }
    }
}
