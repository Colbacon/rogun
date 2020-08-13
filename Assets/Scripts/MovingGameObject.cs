using UnityEngine;
using System.Collections;

public abstract class MovingGameObject : MonoBehaviour
{

    public float moveTime = 0.01f ; //Time taken to move one tile 0.1
    
    public LayerMask collisionLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private float inverseMoveTime;
    protected bool isMoving;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inverseMoveTime = 1f / moveTime;
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

    protected virtual bool Move(Vector3 direction)
    {
        Vector3 initPosition = transform.position;
        Vector3 targetPosition = initPosition + direction;

        RaycastHit2D hit = CheckPath(initPosition, targetPosition);

        if (hit.collider)
            return false;

        UpdateAnimator(direction);
        StartCoroutine(SmoothMovement(targetPosition));
        return true;
    }

    protected IEnumerator SmoothMovement(Vector3 targetPosition)
    {
        isMoving = true;
        
        float sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude; //better performance than vector3.distance
        while (sqrRemainingDistance > float.Epsilon) //epsilon is almost 0
        {
            
            //Vector3 position = Vector3.MoveTowards(rigidBody.position, targetPosition, inverseMoveTime * Time.deltaTime);
            Vector3 position = Vector3.MoveTowards(rigidBody.position, targetPosition, inverseMoveTime * Time.deltaTime);
            rigidBody.MovePosition(position);
            
            sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude;

            yield return null;
        }
        UpdateAnimator(new Vector3(0, 0, 0));
        isMoving = false;
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

    protected void UpdateAnimator(Vector3 direction)
    {
        animator.SetInteger("WalkX", direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0);
        animator.SetInteger("WalkY", direction.y < 0 ? -1 : direction.y > 0 ? 1 : 0);
        
        Debug.Log("-----------------------------");
        Debug.Log("walkx" + (direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0));
        Debug.Log("walky" + (direction.y < 0 ? -1 : direction.y > 0 ? 1 : 0));
    }

}
