using UnityEngine;

namespace Login.HandleScene {
    public class LoginSceneHandler : MonoBehaviour {
        public static void SwitchToCharSelectScene() {
            LoginSceneLoader.Load(SceneEnum.CharSelectScene);
        }
    }
}