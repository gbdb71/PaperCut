using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{

    public float duration;
    float startTime;
    Image img;
    Color startCol;

    private void Awake()
    {
        img = GetComponent<Image>();
        startCol = img.color;
    }

    public void Fade() {
        startTime = Time.time;
        StartCoroutine(FadeImg());
    }

    IEnumerator FadeImg() {
        while (startTime + duration > Time.time) {
            img.color = startCol * (1-(Time.time - startTime) / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
