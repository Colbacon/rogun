using UnityEngine;

public class Patroller : Enemy
{
    public int range;

    Vector3Int position1;
    Vector3Int position2;
    Vector3Int patrolTargetPosition;

    protected override void Start()
    {
        position1 = Vector3Int.RoundToInt(transform.position);
        _Tile tile = GameManager.instance.boardScript.GetRandomFloorTile();
        position2 = tile.GetPosition();
        patrolTargetPosition = position2;

        range = 3;

        base.Start();
    }

    public override void EnemyTurn(bool outOfCamera)
    {
        Vector3 nextPosition;
        Vector3 direction;

        if (!IsTargetInRange(Vector3Int.RoundToInt(player.position), range))
        {
            if (transform.position == patrolTargetPosition) //has reached target position, switching it
                patrolTargetPosition = (patrolTargetPosition == position1) ? position2 : position1;

            nextPosition = GetNextPositionPathfinding(transform.position, patrolTargetPosition, true);
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
