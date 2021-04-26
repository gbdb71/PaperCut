using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public float fadeTime;
    public bool activated;
    float startTime;
    MeshRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    public override void Activate() {
        if (!activated)
        {
            activated = true;
            StartCoroutine(OpenAnimation());
        }
    }
    IEnumerator OpenAnimation() {
        Color col = sr.material.color;
        Color desired = Color.clear;
        startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            sr.material.color = Color.Lerp(col, desired, (Time.time - startTime) / fadeTime);
            Color colour = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            colour.a = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeTime);
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = colour;
            yield return null;
        }
        GetComponent<Collider2D>().enabled = false;
    }
}
