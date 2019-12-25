using UnityEngine;

/// <summary>
/// Sets the specified screen orientation. This object is mostly used for adjusting screen
/// orientation on mobile devices based on the currently displayed Unity scene.
/// </summary>
public class ScreenOrientationSetter : MonoBehaviour {
    [SerializeField] private ScreenOrientation orientation;
    
    void Awake() {
        Screen.orientation = orientation;
        Destroy(gameObject);
    }
}
