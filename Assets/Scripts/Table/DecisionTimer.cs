using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Table {
    public class DecisionTimer : MonoBehaviour {
        [SerializeField] private float timeToDecideInSeconds;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private UnityEvent onTimeUp;

        public bool IsMyTurn { get; set; }
        private float timeLeft;

        private void Awake() {
            Restart();
        }

        void Update() {
            if (timeLeft < 0f) {
                timerText.text = "Time's up!";

                if (IsMyTurn) {
                    onTimeUp?.Invoke();
                }
            }
            else {
                timeLeft -= Time.deltaTime;
                timerText.text = timeLeft.ToString("0.0") + "s";
            }
        }

        public void Show() {
            enabled = true;
        }

        public void Hide() {
            timerText.text = string.Empty;
            enabled = false;
        }

        public void Restart() {
            timeLeft = timeToDecideInSeconds;
        }
    }
}
