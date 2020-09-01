using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject defaultOption;

    GameObject lastSelected;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

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

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;

        AudioManager.instance.Play("MenuClose");
    }

    public void AbandonRun()
    {
        //invoke gameover
        Debug.Log("GameOver");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;

        AudioManager.instance.Play("MenuOpen");

        EventSystem.current.SetSelectedGameObject(null);
        //highlight default option
        EventSystem.current.SetSelectedGameObject(defaultOption);

    }
}
