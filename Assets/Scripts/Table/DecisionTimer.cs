using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Table {
    public class DecisionTimer : MonoBehaviour {
        [SerializeField] private float timeToDecideInSeconds;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private UnityEvent onTimeUp;

        private float timeLeft;

        private void Awake() {
            Reset();
        }

        void Update() {
            if (timeLeft < 0f) {
                timerText.text = "Time's up!";
                onTimeUp?.Invoke();
            }
            else {
                timeLeft -= Time.deltaTime;
                timerText.text = timeLeft.ToString("0.0") + "s";
            }
        }

        public void Reset() {
            timeLeft = timeToDecideInSeconds;
        }
    }
}
