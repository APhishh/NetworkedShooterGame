using System.Data.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Gun,
        Melee,
        Heal
    }
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public float weight;
    public Sprite sprite;
}
