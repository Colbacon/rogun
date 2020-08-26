using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{

    public Text useItemText;
    public Text dropItemText;

    Item item;

    public void AddItem (Item item)
    {
        this.item = item;
        useItemText.text = item.name;
        dropItemText.text = "Drop";
    }

    public void ClearSlot()
    {
        item = null;
        useItemText.text = string.Empty;
        dropItemText.text = string.Empty;
    }

    public void UseItem()
    {
        if (item)
        {
            item.Use();
            
            if (item.useDialogue != null)
            {
                //Debug.Log(item.useDialogue.Length);
                DialogManager.instance.StartDialogue(item.useDialogue);
            }

            Inventory.instance.Remove(item);

            FindObjectOfType<InventoryUI>().CloseInventory();
        }
    }

    public void RemoveItem()
    {
        if (item)
        {
            DialogManager.instance.StartDialogue(new string[1] { "You dropped " + item.name });
            Inventory.instance.Remove(item);
        }
    }
}
