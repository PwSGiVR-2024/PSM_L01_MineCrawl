using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DynamicInventory : ScriptableObject
{
    public int maxItems = 5;
    public List<ItemSlot> items = new(); // Lista slotów, zamiast pojedynczych przedmiotów
    [SerializeField] int stack = 2;
    public bool AddItem(ItemData itemToAdd, GameObject itemObject = null)
    {
        // SprawdŸ, czy istnieje slot z tym samym przedmiotem, który mo¿e stackowaæ
        foreach (var slot in items)
        {
            if (slot.item != null && slot.item == itemToAdd && slot.count < stack)
            {
                slot.count++;
                if (itemObject != null)
                {
                    Destroy(itemObject);
                }
                return true;
            }
        }

        // Dodaj nowy slot, jeœli jest miejsce
        if (items.Count < maxItems)
        {
            items.Add(new ItemSlot(itemToAdd, 1));
            if (itemObject != null)
            {
                Destroy(itemObject);
            }
            return true;
        }

        Debug.Log("No space in the inventory");
        return false;
    }
    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item != null && items[i].item.itemName == item.itemName)
            {
                // Zmniejsz licznik w slocie
                items[i].count--;

                // Usuñ slot, jeœli licznik spadnie do zera
                if (items[i].count <= 0)
                {
                    items.RemoveAt(i);
                }

                Debug.Log($"Usuniêto przedmiot: {item.itemName}");
                return; // WyjdŸ z pêtli po usuniêciu przedmiotu
            }
        }

        Debug.LogWarning($"Przedmiot {item.itemName} nie zosta³ znaleziony w ekwipunku.");
    }
    public void DropItem(ItemData item, Vector3 where)
    {
        // Creates a new object and gives it the item data
        GameObject droppedItem = Instantiate(item.model, where, Quaternion.identity);



    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemData item; // Przedmiot w slocie
    public int count; // Liczba przedmiotów w slocie

    public ItemSlot(ItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}