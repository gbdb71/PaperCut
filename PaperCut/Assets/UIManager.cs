using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private string mode = "";
    public static UIManager main;
    public bool cameraState = false;
    public float camAnimationDuration;
    public int cutsRemaining;
    public bool overGround = true;
    GameObject[] uiButtons;
    float camSize;
    private void Awake()
    {
        if (main != null & main != this) Destroy(this.gameObject);
        else main = this;
        uiButtons = GameObject.FindGameObjectsWithTag("CutButtons");
        foreach (GameObject i in uiButtons)
        {
            i.SetActive(cameraState);
        }
        camSize = Camera.main.orthographicSize;
    }

    public void SetMode(string input) {
        mode = input;
    }
    public string GetMode() {
        return mode;
    }

    public void CameraChange() {
        cameraState = !cameraState;
        print("ASDF");
        if (!cameraState)
        {
            StartCoroutine(AnimateCameraIn());
        }
        else {
            StartCoroutine(AnimateCameraOut());
        }
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Cuttable");
        print(objs.Length);
        foreach (GameObject i in objs)
        {
            Rigidbody2D rb;
            if (i.TryGetComponent(out rb))
            {
                if (!cameraState)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation ;
                }
            }
        }
        if (!cameraState) mode = "";
        foreach (GameObject i in uiButtons)
        {
            i.SetActive(cameraState);
        }
    }

    IEnumerator AnimateCameraIn() {

        List<GameObject> pages = GameObject.FindGameObjectsWithTag("Cuttable").ToList();
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 startPos = Camera.main.transform.position;
        Vector3 desiredPos = playerPos - new Vector3(0, 5, 10);

        float desiredX = 330;
        float currX = Camera.main.transform.eulerAngles.x;
        float startTime = Time.time;

        float camStartSize = Camera.main.orthographicSize;

        while (Time.time < startTime + camAnimationDuration)
        {
            float percThrough = (Time.time - startTime) / camAnimationDuration;
            Camera.main.transform.eulerAngles = (Vector3.right * Mathf.Lerp(currX, desiredX, percThrough));
            Camera.main.transform.position = Vector3.Lerp(startPos, desiredPos, percThrough);
            Camera.main.orthographicSize = Mathf.Lerp(camStartSize, camSize, percThrough);
            yield return null;
        }
    }

    IEnumerator AnimateCameraOut()
    {
        List<GameObject> pages = GameObject.FindGameObjectsWithTag("Cuttable").ToList();
        List<GameObject> newpages = new List<GameObject>();
        foreach (GameObject i in pages) if (i.transform.parent == null) newpages.Add(i);
        pages = newpages;
        print(pages.Count);
        Vector2 center = Vector3.zero;
        foreach (GameObject i in pages) center += (Vector2)i.transform.position;
        center /= pages.Count;

        Vector4 bounds = Vector4.zero;
        foreach (GameObject i in pages) {
            PolygonCollider2D poly = i.GetComponent<PolygonCollider2D>();
            Vector2 maxCorner = poly.bounds.center + poly.bounds.extents;
            Vector2 minCorner = poly.bounds.center - poly.bounds.extents;
            if (maxCorner.x > bounds.x) bounds.x = maxCorner.x;
            if (maxCorner.y > bounds.y) bounds.y = maxCorner.y;
            if (minCorner.x < bounds.z) bounds.z = minCorner.x;
            if (minCorner.y < bounds.w) bounds.w = minCorner.y;
        }

        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 startPos = Camera.main.transform.position;
        Vector3 desiredPos = (Vector3)center - Vector3.forward*10;

        float desiredX = 360;
        float currX = Camera.main.transform.eulerAngles.x;
        float startTime = Time.time;

        Vector2 deltas = new Vector2(bounds.x - bounds.z, bounds.y - bounds.w);
        float ratio = 1;//Camera.main.pixelHeight/Camera.main.pixelWidth;
        

        float cameraTargetSize;
        float cameraStartSize = Camera.main.orthographicSize;
        if (deltas.x > deltas.y) cameraTargetSize = deltas.x * ratio;
        else cameraTargetSize = deltas.y * ratio;

        while (Time.time < startTime + camAnimationDuration)
        {
            float percThrough = (Time.time - startTime) / camAnimationDuration;
            Camera.main.transform.eulerAngles = (Vector3.right * Mathf.Lerp(currX, desiredX, percThrough));
            Camera.main.transform.position = Vector3.Lerp(startPos, desiredPos, percThrough);
            Camera.main.orthographicSize = Mathf.Lerp(cameraStartSize, cameraTargetSize, percThrough);
            yield return null;
        }
    }
}
