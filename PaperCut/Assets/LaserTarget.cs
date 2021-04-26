using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public Interactable target;
    public Material activated, deactivated;
    public GameObject statusLight;
    MeshRenderer mr;
    bool on = false;
    // Start is called before the first frame update
    void Start()
    {
        mr = statusLight.GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if (on)
        {
            mr.material = activated;
            on = false;
        }
        else mr.material = deactivated;
    }

    public void Activate() {
        target.Activate();
        on = true;
    }
}
