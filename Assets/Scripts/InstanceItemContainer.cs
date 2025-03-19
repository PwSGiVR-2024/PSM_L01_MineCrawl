using UnityEngine;

public class InstanceItemContainer : MonoBehaviour
{
    public ItemInstance item;

    public (ItemInstance, GameObject) TakeItem()
    {

        return (item, gameObject);
    }
    private void Start()
    {
        if (item.itemType)
        {
            item.itemName = item.itemType.itemName;
            item.model = item.itemType.model;
            item.description = item.itemType.description;
            item.icon = item.itemType.icon;
        }
    }
}