﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
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

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)){
            animator.SetTrigger("Attack");
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
