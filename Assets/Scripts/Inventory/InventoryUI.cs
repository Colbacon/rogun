using UnityEngine.EventSystems;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public static bool openedInventory = false;

    public GameObject inventoryUI;
    public Transform itemsParent;

    public GameObject defaultSlotOption;

    Inventory inventory;

    InventorySlot[] slots;

    
    GameObject lastSelected;


    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI; //Subscribe to the callback

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        UpdateUI(); //if is called when restarted fron another level
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {

            //inventoryUI.SetActive(!inventoryUI.activeSelf);
            if (!openedInventory)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }

    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        openedInventory = true;

        AudioManager.instance.Play("MenuOpen");

        EventSystem.current.SetSelectedGameObject(null);
        //highlight default option
        EventSystem.current.SetSelectedGameObject(defaultSlotOption);
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
        openedInventory = false;

        AudioManager.instance.Play("MenuClose");
    }

    //called using a delegate on the Inventory
    void UpdateUI()
    {
        //Debug.Log("Callback in action");
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

}
