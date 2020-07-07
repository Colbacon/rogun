using UnityEngine;
using System.Collections;

public abstract class MovingGameObject : MonoBehaviour
{

    public float speed = 10f;
    public int healthPoints = 10;
    public int attackPoints = 10;
    public LayerMask collisionLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;

    protected bool isMoving;

    protected virtual void Start()
    {        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
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

        //RaycastHit2D hit = CheckPath(initPosition, direction);
        RaycastHit2D hit = CheckPath(initPosition, targetPosition);

        //Debug.Log(hit.transform);
        if (hit.collider)
        {
            //Debug.Log("CantMove");
            return false;
        }
        StartCoroutine(SmoothMovement(targetPosition));
        return true;
    }

    protected IEnumerator SmoothMovement(Vector3 targetPosition)
    {
        //Debug.Log("StartMoving");
        isMoving = true;

        float sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude; //better performance than vector3.distance
        //Debug.Log(targetPosition);
        while (sqrRemainingDistance > float.Epsilon) //epsilon is almost 0
        {
            
            Vector3 position = Vector3.MoveTowards(rigidBody.position, targetPosition, speed * Time.deltaTime);
            rigidBody.MovePosition(position);
            
            sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude;

            yield return null;
        }
        //Debug.Log("StopMoving");
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
        //Debug.Log("hit" + hit.transform);
        boxCollider.enabled = true;

        return hit;
    }
}
