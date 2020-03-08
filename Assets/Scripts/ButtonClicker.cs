using UnityEngine;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private KeyCode buttonClickKey;

    void Update()
    {
        if (Input.GetKeyDown(buttonClickKey))
        {
            button.onClick.Invoke();
        }
    }
}
