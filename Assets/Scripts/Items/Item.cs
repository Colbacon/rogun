using UnityEngine;

public class Item : ScriptableObject
{
    new public string name;
    public Sprite sprite;

    [TextArea(3, 10)]
    public string description;

    [TextArea(3, 10)]
    public string[] useDialogue;

    public virtual void Use()
    {
        Debug.Log("Using: "+ name);
    }
}
