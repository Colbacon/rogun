 using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private Transform player;

    protected override void Start()
    {
        GameManager.instance.AddEnemy(this);
        player = Player.instance.transform;

        maxHealthPoints = 5;
        healthPoints = maxHealthPoints;
        attackPoints = 1;

        base.Start();
    }

    public void EnemyTurn(bool outOfCamera)
    {
        Tile start = GameManager.instance.boardScript.GetTile(transform.position);
        Tile end = GameManager.instance.boardScript.GetTile(player.position);
        Debug.Log(player.position);

        List<Tile> path = Pathfinding.AStartSorthestPath(start, end);

        if (path == null) //path to player not found, don't perform action
            return;

        //Only for debugging pourpouses
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y), Color.red, 2);
        }

        Vector3 nextPosition = path[1].GetPosition();

        Vector3 direction = GetDirection(nextPosition);

        if (outOfCamera)
            TelePort(nextPosition, direction);
        else
        {
            bool hasMoved = base.Move(direction);
            if (!hasMoved)
            {
                UpdateAnimatorFacing(direction);
                this.direction = direction; //update where enemy is facing
                AudioManager.instance.Play("EnemyAttack"); //TODO: Move inside Attack method
                Attack<Player>();
            }
        }
    }


    private Vector3 GetDirection(Vector3 targetPosition)
    {
        //try to move
        if (Mathf.Abs(targetPosition.x - transform.position.x) < float.Epsilon)
            direction = targetPosition.y > transform.position.y ? Vector3.up : Vector3.down;
        else
            direction = targetPosition.x > transform.position.x ? Vector3.right : Vector3.left;

        return direction;
    }

    protected override void DealDamage<T>(T component)
    {
        Player player = component as Player;
        player.TakeDamage(attackPoints);
    }

    protected override void Die()
    {
        GameManager.instance.RemoveEnemy(this);
        base.Die();
    }
}
