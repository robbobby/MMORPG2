using Firebase;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Server.Firebase {
    public class FirebaseInit : MonoBehaviour {
        public UnityEvent onFirebaseInitialised = new UnityEvent();

        private void Start() {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (task.Exception != null) {
                    Debug.LogError($"Failed to initialise Firebase with {task.Exception}");
                    return;
                }

                onFirebaseInitialised.Invoke();
            });
        }
    }
}
