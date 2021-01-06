using UnityEngine;
namespace Login {
    public class UIManager : MonoBehaviour {
        public static UIManager instance;
        
        public GameObject loginUI;
        public GameObject registerUI;
        private void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != null) {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }
        public void LoginScreen() { //Functions to change the login screen UI
            loginUI.SetActive(true);
            registerUI.SetActive(false);
        }
        public void RegisterScreen() { // Register button
            loginUI.SetActive(false);
            registerUI.SetActive(true);
        }
    }
}