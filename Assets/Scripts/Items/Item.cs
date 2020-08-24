using UnityEngine;

public class Item : ScriptableObject
{
    new public string name;
    public string description;
    public Sprite sprite;

    public virtual void Use()
    {
        Debug.Log("Using: "+ name);
    }
}
