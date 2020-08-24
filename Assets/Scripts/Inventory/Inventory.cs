using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    //Callback triggered when adding/removing item
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 10;

    public List<Item> items = new List<Item>();

    public bool Add (Item item)
    {
        if (items.Count >= space)
            return false;

        items.Add(item);

        Debug.Log("Added " + item.name);
        //Triggering callback
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();

        return true;
    }

    public void Remove (Item item)
    {
        items.Remove(item);

        //Trigerring callback
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

}
