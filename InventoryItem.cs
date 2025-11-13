using UnityEngine;

[System.Serializable]
public class InventoryItem
{

    // item name/quantity wtv
    public string itemName;
    public GameObject prefab;
    public int quantity;

    public InventoryItem(string name, GameObject obj, int qty = 1)
    {
        itemName = name;
        prefab = obj;
        quantity = qty;
    }
}
