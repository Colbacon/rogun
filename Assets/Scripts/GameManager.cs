using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float startLevelDelay= 2f;
    public float turnDelay = 0.1f;
    //public BoardManager boardScript;
    public Board boardScript;
    public static GameManager instance = null;
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

        enemies = new List<Enemy>();
        boardScript = GetComponent<Board>();

        InitGame();
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
        //for debugging
        /*
        GameObject canvasLevelTrans = GameObject.Find("CanvasLevelTransition");
        canvasLevelTrans.SetActive(true);
        */
        //levelImage.SetActive(true); //delete
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", startLevelDelay);


        Debug.Log("Setting up level " + level + "- GetInstanceID: " + gameObject.GetInstanceID());
        boardScript.BoardSetUp();
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    private void Update()
    {
        
        if (playersTurn || enemiesMoving || doingSetup)
            //Debug.Log(" p: " + playersTurn  + " e: "+ enemiesMoving + " s: " + doingSetup);
            return;
        StartCoroutine(MoveEnemies());
        
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(0.2f);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();

            //yield return new WaitForSeconds(1f / enemies[i].speed);
            yield return new WaitForSeconds(enemies[i].moveTime + 0.2f);
        }

        yield return new WaitForSeconds(0.1f); //colchonsito de seguridad
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
}