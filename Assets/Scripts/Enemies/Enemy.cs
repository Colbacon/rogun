 using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    protected Transform player;

    protected override void Start()
    {
        GameManager.instance.AddEnemy(this);
        player = Player.instance.transform;

        maxHealthPoints = 5;
        healthPoints = maxHealthPoints;
        attackPoints = 1;

        base.Start();
    }

    public virtual void EnemyTurn(bool outOfCamera){}

    protected Vector3 GetNextPositionPathfinding(Vector3 start, Vector3 end, bool ignoreOccupiedTiles = false)
    {
        Tile startTile = GameManager.instance.boardScript.GetTile(start);
        Tile endTile = GameManager.instance.boardScript.GetTile(end);
        //Debug.Log(player.position);

        List<Tile> path = Pathfinding.AStartSorthestPath(startTile, endTile, ignoreOccupiedTiles);

        if (path == null) //path to player not found, don't perform action
            return Vector3.zero;

        //Only for debugging pourpouses
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y), Color.red, 2);
        }

        return path[1].GetPosition();
    }
    
    protected Vector3 GetDirection(Vector3 targetPosition)
    {
        //try to move
        if (Mathf.Abs(targetPosition.x - transform.position.x) < float.Epsilon)
            direction = targetPosition.y > transform.position.y ? Vector3.up : Vector3.down;
        else
            direction = targetPosition.x > transform.position.x ? Vector3.right : Vector3.left;

        return direction;
    }

    protected bool IsTargetInRange(Vector3Int targetPosition, int range)
    {
        Vector3Int pos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        return (Vector3Int.Distance(pos, targetPosition) <= range);
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
