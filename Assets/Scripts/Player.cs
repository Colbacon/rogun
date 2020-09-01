using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{

    #region Singleton

    public static Player instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        Debug.Log("entro awake");
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public float restartLevelDelay = 1f;

    protected override void Start()
    {
        Debug.Log("entro start");
        maxHealthPoints = 10;
        healthPoints = maxHealthPoints;
        attackPoints = 1;

        base.Start();
    }

    void Update()
    {
        //Debug.Log("UPdate");
        if (!GameManager.instance.playersTurn) return;
        if (InventoryUI.openedInventory || PauseMenu.gameIsPaused) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)){
            animator.SetTrigger("Attack");
            AudioManager.instance.Play("PlayerAttack");
            this.TakeDamage(2); 
            Attack <Enemy>();
            //end player's turn if it moved
            GameManager.instance.playersTurn = false;// controlar esto mejor. Ahora solo está en pla debug
        }

        if (horizontal != 0)    //avoid diagonal movement
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            if (!base.isMoving) //really needed?
            {
                Vector3 direction = new Vector3(horizontal, vertical);
                bool hasMoved = base.Move(direction);
                if (!hasMoved)
                {
                    //Debug.Log("Going to face");
                    UpdateAnimatorFacing(direction);
                    this.direction = direction; //update where player is facing
                }
                else
                {
                    //end player's turn if it moved
                    GameManager.instance.playersTurn = false;
                }
            }
        }
    }


    protected override void DealDamage<T>(T component)
    {
        Enemy enemy = component as Enemy;

        enemy.TakeDamage(attackPoints);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Ladder")
        {
            Invoke("RestartScene", restartLevelDelay);
        }

        if(collider.tag == "Item")
        {
            Item item = collider.gameObject.GetComponent<ItemDataAssigner>().item;

            if (Inventory.instance.Add(item))
                Destroy(collider.gameObject);
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
