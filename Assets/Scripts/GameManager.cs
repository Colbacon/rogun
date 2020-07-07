using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float startLevelDelay= 2f;
    public BoardManager boardScript;
    public static GameManager instance = null;
    public float turnDelay = 0.2f;
    public bool playersTurn = true;

    private GameObject levelImage;
    private Text levelText;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();

        enemies = new List<Enemy>();

        InitGame();
    }



    private void Update()
    {
        /*
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
        */
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", startLevelDelay);


        Debug.Log("Setting up level " + level + "- GetInstanceID: " + gameObject.GetInstanceID());
        boardScript.SetupScene();
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    /*
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();

            yield return new WaitForSeconds(1f/enemies[i].speed);
        }
        playersTurn = true;

        enemiesMoving = false;
    }
    */
}