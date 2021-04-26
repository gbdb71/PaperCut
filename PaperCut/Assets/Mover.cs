using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Mover : MonoBehaviour
{
    private static Mover mover;
    public static Mover main { get { return mover; } }
    public LayerMask paperMask;

    public LayerMask blockMask;

    public Rigidbody2D current;
    public bool moving = false;
    Vector2 lastPos;
    public Vector2 localDelta;

    private void Awake()
    {
        if (mover != null & mover != this) Destroy(this.gameObject);
        else mover = this;
    }
    private void Update()
    {
        if (UIManager.main.GetMode() == "move")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetButtonDown("Fire1"))
            {
                RaycastHit2D hit;
                if (hit = Physics2D.Raycast(mousePos, Vector2.zero, 1, paperMask))
                {
                    current = hit.collider.attachedRigidbody;
                    localDelta = mousePos - (Vector2)current.transform.position;
                    moving = true;
                    current.drag = 0;
                }
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                if (moving)
                {
                    current.velocity = Vector2.zero;
                    current.drag = 100;
                    current = null;
                    moving = false;
                }
            }
            if (Input.GetButton("Fire1"))
            {
                if (moving)
                {
                    RaycastHit2D hit;
                    Vector2 delta = (mousePos - (Vector2)current.transform.position - localDelta);
                    if (hit = Physics2D.Raycast((Vector2)current.transform.position + localDelta, delta.normalized, delta.magnitude, blockMask)) {
                        if (hit.collider.tag == "CutBlocker") {
                            current.velocity = Vector2.zero;
                            current.drag = 100;
                            current = null;
                            moving = false;
                            return;
                        }
                    }
                    else if (hit = Physics2D.Raycast((Vector2)current.transform.position + localDelta + (Vector2)transform.up * current.GetComponent<Collider2D>().bounds.extents.y, delta.normalized, delta.magnitude, blockMask))
                    {
                        if (hit.collider.tag == "CutBlocker")
                        {
                            current.velocity = Vector2.zero;
                            current.drag = 100;
                            current = null;
                            moving = false;
                            return;
                        }
                    }
                    else if (hit = Physics2D.Raycast((Vector2)current.transform.position + localDelta - (Vector2)transform.up * current.GetComponent<Collider2D>().bounds.extents.y, delta.normalized, delta.magnitude, blockMask))
                    {
                        if (hit.collider.tag == "CutBlocker")
                        {
                            current.velocity = Vector2.zero;
                            current.drag = 100;
                            current = null;
                            moving = false;
                            
                            return;
                        }
                    }
                    float mouseSpeed = (mousePos - lastPos).magnitude;
                    current.velocity = delta / Time.deltaTime;
                    lastPos = mousePos;
                }
            }
        }
    }

}