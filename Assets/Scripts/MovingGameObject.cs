using UnityEngine;
using System.Collections;

public abstract class MovingGameObject : MonoBehaviour
{

    public float speed = 0.1f;
    public LayerMask collisionLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;

    private bool isMoving;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
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

        RaycastHit2D hit = CheckPath(initPosition, direction);

        if (hit.transform != null)
        {
            Debug.Log("CantMove");
            return false;
        }
        StartCoroutine(SmoothMovement(targetPosition));
        return true;
    }

    protected IEnumerator SmoothMovement(Vector3 targetPosition)
    {
        Debug.Log("StartMoving");
        isMoving = true;

        float sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude; //better performance than vector3.distance

        while (sqrRemainingDistance > float.Epsilon) //epsilon is almost 0
        {
            Vector3 position = Vector3.MoveTowards(rigidBody.position, targetPosition, speed * Time.deltaTime);
            rigidBody.MovePosition(position);

            sqrRemainingDistance = (transform.position - targetPosition).sqrMagnitude;

            yield return null;
        }
        Debug.Log("StopMoving");
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

    //return a raycast with the object detected on the path
    protected RaycastHit2D CheckPath(Vector3 position, Vector3 direction)
    {
        //disable the boxcollider so linecast doesn't hit object's own collider
        boxCollider.enabled = false;

        //check if there is a gameobject with boxcollider in the way
        RaycastHit2D hit = Physics2D.Linecast(position, direction, collisionLayer);

        boxCollider.enabled = true;

        return hit;
    }
}
