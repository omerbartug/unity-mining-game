using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string displayName;

    public Sprite icon;

    public int sellPrice;
    public bool sellable;
}