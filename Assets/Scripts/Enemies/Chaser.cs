using UnityEngine;

public class Chaser : Enemy
{
    public override void EnemyTurn(bool outOfCamera)
    {
        Vector3 nextPosition = GetNextPositionPathfinding(transform.position, player.position);
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
}
