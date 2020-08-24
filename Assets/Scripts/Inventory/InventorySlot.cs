using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{

    public Text slotText;

    Item item;

    public void AddItem (Item item)
    {
        this.item = item;
        slotText.text = item.name;
    }

    public void ClearSlot()
    {
        item = null;
        slotText.text = string.Empty;
    }

    public void UseItem()
    {
        if (item)
        {
            item.Use();
            Inventory.instance.Remove(item);
        }
    }

    public void RemoveItem()
    {
        Inventory.instance.Remove(item);
    }
}
