using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingGameObject
{
    public float restartLevelDelay = 1f;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {

        //if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)    //avoid diagonal movement
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            if (!base.isMoving)
            {
                Vector3 direction = new Vector3(horizontal, vertical);
                //Debug.Log("going to move");
                base.Move(direction);
                //GameManager.instance.playersTurn = false;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ladder")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
