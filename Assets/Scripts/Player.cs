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
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public float restartLevelDelay = 1f;

    protected override void Start()
    {
        maxHealthPoints = 10;
        healthPoints = maxHealthPoints;
        attackPoints = 1;

        base.Start();
    }

    void Update()
    {
        if (!GameManager.instance.playersTurn) return;
        if (InventoryUI.openedInventory || PauseMenu.gameIsPaused) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)){
            //TODO: move animation and sfx inside parent method
            animator.SetTrigger("Attack");
            AudioManager.instance.Play("PlayerAttack");
            Attack <Enemy>();
           
            GameManager.instance.playersTurn = false;
        }

        if (horizontal != 0)    //avoid diagonal movement
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            if (!base.isMoving)
            {
                Vector3 direction = new Vector3(horizontal, vertical);
                bool hasMoved = base.Move(direction);
                if (!hasMoved)
                {
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

    protected override void Die()
    {
        base.Die();
        GameManager.instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Ladder")
        {
            enabled = false; //avoid to move player
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
        enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
