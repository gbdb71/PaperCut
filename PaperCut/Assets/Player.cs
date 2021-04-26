using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Animator slash;
    public float speed;
    public float fallTime;
    public LayerMask mask;
    public float dashDist;
    bool dead = false;
    Vector3 storedLocal;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r")) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (!UIManager.main.cameraState)
        {
            storedLocal = Vector3.zero;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist = transform.position.z-ray.origin.z / ray.direction.z;
            Vector2 intersection = ray.origin + ray.direction * dist;
            Vector2 delta = intersection - (Vector2)transform.position;
            transform.up = delta;

            if (!dead && Input.GetButtonDown("Dash") && UIManager.main.GetMode() == "")
            {
                /*slash.gameObject.SetActive(true);
                slash.SetTrigger("slash");*/
                RaycastHit2D hit;
                if (hit = Physics2D.Raycast(transform.position, transform.up, dashDist, mask))
                {
                    print(hit.collider.name);
                    transform.position = Vector3.Scale((Vector3)hit.point - transform.up*0.1f, new Vector3(1, 1, 0)) + Vector3.forward * transform.position.z;
                }
                else {
                    transform.position += transform.up * dashDist;
                }
            }

            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            rb.velocity = movement * speed;
        }
        else
        {
            if (storedLocal == Vector3.zero) storedLocal = transform.localPosition;
            transform.localPosition = storedLocal;
            rb.velocity = Vector2.zero;
        }
        if (!UIManager.main.overGround && !dead) {
            dead = true;
            
            StartCoroutine(fallHole());
        }
    }
    IEnumerator fallHole() {
        Vector3 startSize = transform.lossyScale;
        Vector3 startPos = transform.position;
        Vector3 desiredSize = Vector3.zero;
        Vector3 desiredPos = transform.position + (Vector3)rb.velocity/10 + Vector3.down;
        float startTime = Time.time;
        while (Time.time < startTime + fallTime) {
            transform.parent = null;
            transform.localScale = Vector3.Lerp(startSize, desiredSize, (Time.time - startTime) / fallTime);
            transform.position = Vector3.Lerp(startPos, desiredPos, (Time.time - startTime) / fallTime);
            yield return null;
        }
        StartCoroutine(Die());
    }
    public IEnumerator Die() {
        rb.simulated = false;
        dead = true;
        Destroy(transform.GetChild(0).gameObject);
        Destroy(transform.GetChild(1).gameObject);
        Destroy(transform.GetChild(2).gameObject);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
