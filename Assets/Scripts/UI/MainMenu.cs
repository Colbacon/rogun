using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject defaultOption;
    private GameObject lastSelected;

    void Start()
    {
        AudioManager.instance.Play("MenuTheme");

        EventSystem.current.SetSelectedGameObject(null);
        //highlight default option
        EventSystem.current.SetSelectedGameObject(defaultOption);

        //Play();
    }

    void Update()
    {
        //avoid mouse stole button highlight
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    public void Play()
    {
        AudioManager.instance.Stop("MenuTheme");

        //load next scene in the scene's queue, that's the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
