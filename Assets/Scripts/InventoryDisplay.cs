using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public DynamicInventory inventory;
    public ItemDisplay[] slots;

    private void Start()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].UpdateItemDisplay(inventory.items[i].item.icon, i, inventory.items[i].count);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

}