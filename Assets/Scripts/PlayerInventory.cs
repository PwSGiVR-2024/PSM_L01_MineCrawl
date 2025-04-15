//using UnityEngine;

//public class PlayerInventory : MonoBehaviour
//{
//    public DynamicInventory inventory;
//    public InventoryDisplay display;
//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.TryGetComponent(out InstanceItemContainer foundItem))
//        {
//            var (itemInstance, itemObject) = foundItem.TakeItem();
//            inventory.AddItem(itemInstance.itemType, itemObject);
//            display.UpdateInventory();
//        }
//    }
//    private void OnCollisionEnter(Collision other)
//    {
//        if (other.collider.TryGetComponent(out InstanceItemContainer foundItem))
//        {
//            var (itemInstance, itemObject) = foundItem.TakeItem();
//            inventory.AddItem(itemInstance.itemType, itemObject);
//            display.UpdateInventory();
//        }
//    }
//}