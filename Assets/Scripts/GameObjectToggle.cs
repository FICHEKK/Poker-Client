using UnityEngine;

public class GameObjectToggle : MonoBehaviour {
    [SerializeField] private GameObject gameObjectToToggle;

    public void ShowGameObject() {
        gameObjectToToggle.SetActive(true);
    }
        
    public void HideGameObject() {
        gameObjectToToggle.SetActive(false);
    }
}