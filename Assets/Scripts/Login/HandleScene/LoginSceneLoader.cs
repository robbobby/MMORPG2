using UnityEngine.SceneManagement;

namespace Login.HandleScene {
    public static class LoginSceneLoader {
        public static void Load(SceneEnum scene) {
            SceneManager.LoadScene(scene.ToString());
        }
    }
}