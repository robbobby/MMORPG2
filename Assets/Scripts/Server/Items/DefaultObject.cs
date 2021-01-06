using UnityEngine;

namespace Server.Items {
    [CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
    public class DefaultObject : ItemObject {
        public void Awake() {
            itemType = ItemType.Default;
        }
    }
}