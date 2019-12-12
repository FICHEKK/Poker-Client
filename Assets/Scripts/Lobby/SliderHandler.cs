using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class SliderHandler : MonoBehaviour {
        [SerializeField] private TableData data;
        [SerializeField] private Slider slider;
        private TMP_Text text;

        private void Start() {
            text = GetComponent<TMP_Text>();
        }

        public void UpdateBuyInText() {
            int minBuyIn = data.SmallBlind * 2 * 10;
            int maxBuyIn = data.SmallBlind * 2 * 200;
            int amount = (int) Math.Round(minBuyIn + (maxBuyIn - minBuyIn) * slider.value);
            text.text = "Buy-In: " + amount;
        }
    }
}
