using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{

    public Text useItemText;
    public Image removeItemImage;

    private Item item;

    private InventoryUI inventoryUI;

    private void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();    
    }

    public void AddItem (Item item)
    {
        this.item = item;

        if(useItemText != null && removeItemImage != null)
        {
            useItemText.text = item.name;
            removeItemImage.enabled = true;
        }
    }

    public void ClearSlot()
    {
        item = null;

        if (useItemText != null && removeItemImage != null)
        {
            useItemText.text = string.Empty;
            removeItemImage.enabled = false;
        }
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

            inventoryUI.CloseInventory();
        }
    }

    public void RemoveItem()
    {
        if (item)
        {
            AudioManager.instance.Play("InventoryRemoveItem");
            DialogueManager.instance.StartDialogue(new string[1] { "You dropped " + item.name });

            inventoryUI.CloseInventory();
            Inventory.instance.Remove(item);
        }
    }
}
