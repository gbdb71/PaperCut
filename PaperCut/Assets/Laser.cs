using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Vector3 lineDelta;
    public LayerMask mask;
    public Transform particles;
    public float targetThreshold;
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit;
        Vector3 target;
        Debug.DrawRay(transform.TransformPoint(lineDelta), transform.up*1000, Color.red, Time.deltaTime, false);
        if (hit = Physics2D.Raycast(transform.TransformPoint(lineDelta), transform.up, 1000, mask))
        {

            print("Hit: "+hit.collider.name);
            target.x = hit.point.x;
            target.y = hit.point.y;
            
            if (hit.collider.gameObject.tag == "LaserTarget" && Vector2.Dot(transform.up, hit.collider.transform.up) * -1 > (1-targetThreshold))
            {
                hit.collider.GetComponent<LaserTarget>().Activate();
            }
            else if (hit.collider.gameObject.tag == "Player") {
                hit.collider.GetComponent<Player>().StartCoroutine(hit.collider.GetComponent<Player>().Die());
            }
        }
        else {
            Vector2 newPos = transform.position + transform.up * 1000;
            target.x = newPos.x;
            target.y = newPos.y;
        }
        target.z = transform.TransformPoint(lineDelta).z;
        particles.transform.position = target;
        particles.forward = transform.position - target;
        lr.SetPosition(0, transform.TransformPoint(lineDelta));
        lr.SetPosition(1, target);
    }
}
