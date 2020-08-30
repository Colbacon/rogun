using UnityEngine;
using System.Collections;


public abstract class Character : MonoBehaviour
{

    public int maxHealthPoints;
    public int healthPoints;
    public int attackPoints;

    public HealthBar healthbar;

    protected Vector3 direction;
    public LayerMask collisionLayer;
    public float moveTime = 0.1f; //Time taken to move one tile 0.1
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private float inverseMoveTime;
    protected Animator animator;
    protected bool isMoving;
    protected bool isAttacking;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inverseMoveTime = 1f / moveTime;
        healthbar.SetMaxHealth(maxHealthPoints);
        isMoving = false;
    }

    /**
     * -------------------------------
     * -------------------------------
     *            MOVEMENT 
     * -------------------------------
     * -------------------------------
     *  
     * */

    protected bool Move(Vector3 direction)
    {
        Vector3 initPosition = transform.position;
        Vector3 targetPosition = initPosition + direction;

        RaycastHit2D hit = CheckPath(initPosition, targetPosition);

        if (hit.collider)
            return false;

        this.direction = direction; //update where character is facing

        StartCoroutine(SmoothMovement(targetPosition));

        GameManager.instance.boardScript.SetOccupiedTile(initPosition, false);
        GameManager.instance.boardScript.SetOccupiedTile(targetPosition, true);

        return true;
    }

    protected virtual IEnumerator SmoothMovement(Vector3 targetPosition)
    {
        isMoving = true;
        UpdateAnimatorWalk(direction);

        float sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude; //better performance than vector3.distance
        while (sqrRemainingDistance > float.Epsilon) //epsilon is almost 0
        {
            
            Vector3 position = Vector3.MoveTowards(rigidBody.position, targetPosition, inverseMoveTime * Time.deltaTime);
            rigidBody.MovePosition(position);
            
            sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude;

            yield return null;
        }
        //Make sure the object is exactly at the end of its movement.
        rigidBody.MovePosition(targetPosition);

        UpdateAnimatorWalk(Vector3.zero);
        isMoving = false;
    }

    /**
     * -------------------------------
     * -------------------------------
     *            COMBAT
     * -------------------------------
     * -------------------------------
     *  
     * */

    protected void Attack <T> () //melee attack, range 1
        where T : Component
    {
        Vector3 initPosition = transform.position;
        Vector3 targetPosition = initPosition + direction;

        RaycastHit2D hit = CheckPath(initPosition, targetPosition);

        if (hit.collider)
        {
            T hitComponent  = hit.transform.GetComponent<T>();
            if(hitComponent)
                DealDamage<T>(hitComponent);
        }
    }

    protected abstract void DealDamage<T>(T component)
            where T : Component;

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        healthbar.SetHealth(healthPoints);
        //take damage animation
        //take damage sound

        Debug.Log("Taken damage- hp:" + healthPoints);
        if(healthPoints <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        //reproduce animation diying
        //reproduce sound dying
        Debug.Log("Died");
        this.enabled = false;
        boxCollider.enabled = false;
        Destroy(gameObject);
    }

    public void Heal(int heal)
    {
        healthPoints = (healthPoints + heal) < maxHealthPoints ? (healthPoints + heal) : maxHealthPoints;
        healthbar.SetHealth(healthPoints);
    }

    /**
      * -------------------------------
      * -------------------------------
      *            COMMON
      * -------------------------------
      * -------------------------------
      *  
      * */

    //return a raycast with the object detected on the path
    protected RaycastHit2D CheckPath(Vector2 initPosition, Vector2 targetPosition)
    {
        //disable the boxcollider so linecast doesn't hit object's own collider
        boxCollider.enabled = false;

        Debug.DrawLine(initPosition, targetPosition, Color.red);
        RaycastHit2D hit = Physics2D.Linecast(initPosition, targetPosition, collisionLayer);

        boxCollider.enabled = true;

        return hit;
    }

    /**
      * -------------------------------
      * -------------------------------
      *            COMMON
      * -------------------------------
      * -------------------------------
      *  
      * */

    protected void UpdateAnimatorWalk(Vector3 direction)
    {
        /* animator.SetInteger("WalkX", direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0);
         animator.SetInteger("WalkY", direction.y < 0 ? -1 : direction.y > 0 ? 1 : 0);
         /*
         Debug.Log("-----------------------------");
         Debug.Log("walkx" + (direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0));
         Debug.Log("walky" + (direction.y < 0 ? -1 : direction.y > 0 ? 1 : 0));
         */

        if (direction == Vector3.up)
            animator.SetBool("WalkUp", true);
        if (direction == Vector3.down)
            animator.SetBool("WalkDown", true);
        if (direction == Vector3.right)
            animator.SetBool("WalkRight", true);
        if (direction == Vector3.left)
            animator.SetBool("WalkLeft", true);
        
        if (direction == Vector3.zero)
        {
            animator.SetBool("WalkUp", false);
            animator.SetBool("WalkDown", false);
            animator.SetBool("WalkRight", false);
            animator.SetBool("WalkLeft", false);
        }
    }

    protected void UpdateAnimatorFacing(Vector3 direction)
    {
        if (direction == Vector3.up)
            animator.SetTrigger("FaceUp");            
        if (direction == Vector3.down)
            animator.SetTrigger("FaceDown");
        if (direction == Vector3.right)
            animator.SetTrigger("FaceRight");
        if (direction == Vector3.left)
            animator.SetTrigger("FaceLeft");
    }
}
