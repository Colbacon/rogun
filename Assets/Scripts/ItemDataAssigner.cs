using UnityEngine;

public class ItemDataAssigner : MonoBehaviour
{
    public Item item;

    public SpriteRenderer spriteRenderer;

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.sprite;
    }

}
