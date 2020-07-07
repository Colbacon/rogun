using UnityEngine;

public class Enemy : MovingGameObject
{
    private Animator animator;
    private Transform target;

    protected override void Start()
    {
        GameManager.instance.AddEnemy(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    public void MoveEnemy()
    {
        Vector3 direction;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            direction = target.position.y > transform.position.y ? Vector3.up : Vector3.down;
        }
        else
        {
            direction = target.position.x > transform.position.x ? Vector3.right : Vector3.left;
        }

        base.Move(direction);
        if (!base.Move(direction))
            Debug.Log("Attack");
    }
}
