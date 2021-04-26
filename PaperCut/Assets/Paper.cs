using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    Rigidbody2D rb;
    float startTime;
    float floatTime = 0.5f;
    float breakThresh = 1;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startTime = Time.time;
        StartCoroutine(ActivateDrag());
        rb.drag = 1;

    }
    private void OnMouseEnter()
    {
        if (Cutter.main.cutting && Cutter.main.current == null) Cutter.main.current = gameObject;
        if (UIManager.main.GetMode() == "move") GetComponent<LineRenderer>().enabled = true;
    }

    private void OnMouseExit()
    {
        if (UIManager.main.GetMode() == "move") GetComponent<LineRenderer>().enabled = false;
    }

    
    public IEnumerator ActivateDrag() {
        print("A");
        while (floatTime + startTime > Time.time) yield return null;
        rb.drag = 100;
        print("B");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") collision.transform.parent = transform;
    }
}
