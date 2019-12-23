using UnityEngine;

public class ScreenOrientationSetter : MonoBehaviour {
    [SerializeField] private ScreenOrientation orientation;
    
    void Awake() {
        Screen.orientation = orientation;
    }
}
