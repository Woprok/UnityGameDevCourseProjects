using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D
{
    public class KeyDoor : MonoBehaviour
    {
        public string[] requiredInventoryItemKeys;

        public InventoryController characterInventory;
        public UnityEvent onUnlocked;

        [ContextMenu("Update State")]
        void CheckInventory()
        {
            var stateIndex = -1;
            foreach (var i in requiredInventoryItemKeys)
            {
                if (characterInventory.HasItem(i))
                {
                    stateIndex++;
                }
            }
            if (stateIndex >= 0 && stateIndex == requiredInventoryItemKeys.Length - 1)
            {
                onUnlocked.Invoke();
            }
        }

        void OnEnable()
        {
            characterInventory.OnInventoryLoaded += CheckInventory;
        }

        void Update()
        {
            CheckInventory();
        }
    }
}