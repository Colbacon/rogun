using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{

    public Text useItemText;
    public Image removeItemImage;

    Item item;

    public void AddItem (Item item)
    {
        this.item = item;
        useItemText.text = item.name;
        removeItemImage.enabled = true;

    }

    public void ClearSlot()
    {
        item = null;
        useItemText.text = string.Empty;
        removeItemImage.enabled = false;
    }

    public void UseItem()
    {
        if (item)
        {
            item.Use();
            
            if (item.useDialogue != null)
            {
                AudioManager.instance.Play("InventoryUseItem");
                DialogueManager.instance.StartDialogue(item.useDialogue);
            }

            Inventory.instance.Remove(item);

            FindObjectOfType<InventoryUI>().CloseInventory();
        }
    }

    public void RemoveItem()
    {
        if (item)
        {
            AudioManager.instance.Play("InventoryRemoveItem");
            DialogueManager.instance.StartDialogue(new string[1] { "You dropped " + item.name });
            Inventory.instance.Remove(item);
        }
    }
}
