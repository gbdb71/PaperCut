using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parentifier : MonoBehaviour
{
    public LayerMask mask;
    public bool player;

    // Update is called once per frame
    void Update()
    {
        /*if (UIManager.main.cameraState)
        {*/
            RaycastHit2D hit;
        if (Mover.main.moving == false)
        {
            if (hit = Physics2D.Raycast(transform.position, Vector2.zero, 0.1f, mask))
            {
                if (player)
                {
                    if (UIManager.main.cameraState) transform.parent.parent = hit.transform;
                    else
                    {
                        UIManager.main.overGround = true;
                        transform.parent.parent = null;
                    }
                }
                else transform.parent = hit.transform;
                print(hit.collider.name);
            }
            else if (player)
            {
                transform.parent.parent = null;
                UIManager.main.overGround = false;
            }
            else transform.parent = null;
        }
        //}
    }
}
