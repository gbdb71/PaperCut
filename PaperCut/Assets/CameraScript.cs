using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector3 veloc = Vector3.zero;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIManager.main.cameraState) transform.position = Vector3.SmoothDamp(transform.position, target.transform.position - new Vector3(0,5,10), ref veloc, 0.5f);
    }
}
