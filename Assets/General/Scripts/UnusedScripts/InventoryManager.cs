using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<InventoryItem> items = new List<InventoryItem>();
    public TMP_Text inventoryText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(InventoryItem newItem)
    {
        if (!items.Exists(i => i.id == newItem.id))
        {
            items.Add(newItem);
            UpdateInventoryUI();
        }
    }

    public bool HasItem(string id)
    {
        return items.Exists(i => i.id == id);
    }

    public void RemoveItem(string id)
    {
        items.RemoveAll(i => i.id == id);
        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        if (inventoryText == null) return;
        inventoryText.text = "Inventario:\n";
        foreach (var item in items)
        {
            inventoryText.text += "- " + item.displayName + "\n";
        }
    }
}