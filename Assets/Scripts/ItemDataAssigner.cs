using UnityEngine;

public class ItemDataAssigner : MonoBehaviour
{
    public Item item;

    public SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer.sprite = item.sprite;
    }

}
