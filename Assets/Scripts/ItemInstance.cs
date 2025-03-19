using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemInstance
{
    public ItemData itemType;
    public string itemName;
    public Sprite icon;
    public GameObject model;
    public string description;
    public ItemInstance(ItemData itemData)
    {
        itemType = itemData;
        itemName = itemData.itemName;
        icon = itemData.icon;
        model = itemData.model;

    }
}