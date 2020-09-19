using UnityEngine;

public class Pawn : Enemy
{
    public int range;

    protected override void Start()
    {
        range = 3;
        base.Start();
    }

    public override void EnemyTurn(bool outOfCamera)
    {
        Vector3 nextPosition;
        Vector3 direction;

        if (!IsTargetInRange(Vector3Int.RoundToInt(player.position), range))
        {
            Tile tile = GameManager.instance.boardScript.GetTile(transform.position);
            nextPosition = tile.reachableNeighbours[Random.Range(0, tile.reachableNeighbours.Count)].GetPosition();
        }
        else
        {
            nextPosition = GetNextPositionPathfinding(transform.position, player.position);
        }
        direction = GetDirection(nextPosition);

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
}
