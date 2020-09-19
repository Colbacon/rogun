using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float levelTransitionDelay= 2f;
    public float turnDelay = 0.1f;
    public Board boardScript;
    public static GameManager instance = null;
    public bool playersTurn = true;

    private GameObject levelImage;
    private Text levelText;
    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingLevelSetup;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded; //subscribe CallBack to sceneLoaded event

        AudioManager.instance.Play("Theme");


        enemies = new List<Enemy>();
        boardScript = GetComponent<Board>();
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    void InitGame()
    {
        //Debug.Log("Setting up level " + level + "- GetInstanceID: " + gameObject.GetInstanceID());

        doingLevelSetup = true;

        ShowLevelTransition();

        enemies.Clear();

        var watch = System.Diagnostics.Stopwatch.StartNew();

        boardScript.BoardSetUp();

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.LogWarning("Board setup time: " + elapsedMs);
    }

    void ShowLevelTransition()
    {
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);

        Invoke("HideLevelTransition", levelTransitionDelay);
    }

    void HideLevelTransition()
    {
        levelImage.SetActive(false);
        doingLevelSetup = false;
    }

    private void Update()
    {
        
        if (playersTurn || enemiesMoving || doingLevelSetup)
            return;
        StartCoroutine(EnemiesTurn());
        
    }

    private IEnumerator EnemiesTurn()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(Player.instance.moveTime + turnDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (IsOutOfCamera(enemies[i].transform))
            {
                enemies[i].EnemyTurn(true);
                //Debug.LogWarning(enemies[i].transform.position + " is out of camera");
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                enemies[i].EnemyTurn(false);
                yield return new WaitForSeconds(enemies[i].moveTime + turnDelay);
            }
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public bool IsOutOfCamera (Transform target)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(target.position);
        return !(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0);
    }

    public void GameOver()
    {
        AudioManager.instance.Stop("Theme");

        levelText.text = "GAME OVER";
        levelImage.SetActive(true);

        //Undo dontDestroyOnLoad, to destroy automatically these gameObjects when change to main menu scene
        //https://answers.unity.com/questions/1491238/undo-dontdestroyonload.html
        SceneManager.MoveGameObjectToScene(Inventory.instance.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(Player.instance.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        Invoke("LoadMainMenu", levelTransitionDelay);   
    }

    private void LoadMainMenu()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; //unsuscribe OnSceneLoaded Callback from sceneLoaded event
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1, LoadSceneMode.Single); //Load main menu scene
    }

}