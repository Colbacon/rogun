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

    public void MoveEnemy()
    {
        Vector3 direction;

        /*
        //try to move
        if (Mathf.Abs(player.position.x - transform.position.x) < float.Epsilon)
        {
            direction = player.position.y > transform.position.y ? Vector3.up : Vector3.down;
        }
        else
        {
            direction = player.position.x > transform.position.x ? Vector3.right : Vector3.left;
        }
        */

        ChasePlayer();

        //bool canMove = base.Move(direction);

        //if cant move, attack player
        //base.Attack<Player>();
        
    }

    private void ChasePlayer()
    {
        // the code that you want to measure comes here

        /*
        Tile start = tileMap[rooms[1].x + 2][rooms[1].y + 2];
        Tile end = tileMap[rooms[3].x + 6][rooms[3].y + 6];

        var watch = System.Diagnostics.Stopwatch.StartNew();
        List<Tile> path = Pathfinding.AStartPathfinding(start, end);
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("----------TIme: " + elapsedMs);
        if (path == null)
            Debug.Log("nullito");
        for (int i = 0; i < path.Count; i++)
        {
            int x = path[i].x;
            int y = path[i].y;
            Instantiate(ladder, new Vector3(x, y, 0f), Quaternion.identity);
            Debug.Log("iteration " + i + "  x: " + x + "  y: " + y);
        }*/
        Tile start = GameManager.instance.boardScript.GetTile(transform.position);
        Tile end = GameManager.instance.boardScript.GetTile(player.position);

        List<Tile> path = Pathfinding.AStartSorthestPath(start, end);

        if (path != null)
        {
            /*Debug.Log("----------------------------");
            Debug.Log(path[0].GetPosition());
            Debug.Log(path[path.Count - 1].GetPosition());
            */
            for (int i = 0; i < path.Count - 1 ; i++)
            {

                Debug.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y), Color.red,2);
            }

            base.Move(path[1].GetPosition());
        }
        else
        {
            Debug.Log("NO SOLUTION"); 
        }

        //base.Move(new Vector3(path[0].x, path[0].y));

        //Tile 

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
