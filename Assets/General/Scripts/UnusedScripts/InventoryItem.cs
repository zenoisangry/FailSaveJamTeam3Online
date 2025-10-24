using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Game/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
}