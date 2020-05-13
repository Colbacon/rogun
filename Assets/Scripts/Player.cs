using UnityEngine;

public class Player : MovingGameObject
{

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)    //avoid diagonal movement
            vertical = 0;
        /*
        if (horizontal != 0 || vertical != 0)
            AttemptMove(horizontal, vertical);*/
    }
}
