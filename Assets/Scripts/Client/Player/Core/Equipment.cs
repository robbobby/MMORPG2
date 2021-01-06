using UnityEngine;

namespace Client.Player.Core {
    public class Equipment : MonoBehaviour {
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private Transform weaponTransform;
        private void  Start() {
            weaponPrefab = null;
            weaponTransform = null;
        }
    }
}