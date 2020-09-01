using UnityEngine.EventSystems;
using UnityEngine;

public class Button : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        AudioManager.instance.Play("UISelectButton");
    }
}
