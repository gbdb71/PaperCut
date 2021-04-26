using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Cutter : MonoBehaviour
{
    private static Cutter cutter;
    public static Cutter main { get { return cutter; } }

    public UnityEngine.UI.Button uiButton;

    public LayerMask paperMask;

    public GameObject current;
    public Vector2 start, end;
    public bool cutting = false;

    public Image startImg, endImg;
    public Image lineDot;
    public float lineThreshold;

    public List<Image> lineDots = new List<Image>();

    private void Awake()
    {
        if (cutter != null & cutter != this) Destroy(this.gameObject);
        else cutter = this;
        
    }
    private void Start()
    {
        uiButton.GetComponentInChildren<TextMeshProUGUI>().text = "x" + UIManager.main.cutsRemaining.ToString();
    }
    private void Update()
    {
        if (UIManager.main.GetMode() == "cut" && UIManager.main.cutsRemaining > 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                startImg.gameObject.SetActive(true);
                endImg.gameObject.SetActive(true);
                startImg.transform.position = Input.mousePosition;
                cutting = true;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                cutting = false;
                startImg.gameObject.SetActive(false);
                endImg.gameObject.SetActive(false);
                while (lineDots.Count > 0)
                {
                    lineDots[0].GetComponent<FadeOut>().Fade();
                    lineDots.RemoveAt(0);
                }
                if (current != null && !current.GetComponent<PolygonCollider2D>().OverlapPoint(end))
                {
                    RaycastHit2D[] hits;
                    List<Transform> tests = GetChildrenRecursively(current.transform);
                    GameObject split = Cut(current, start, end);
                    if (split != null)
                    {
                        foreach (Transform i in GetChildrenRecursively(split.transform)) {
                            Destroy(i.gameObject);
                        }

                        List<int> layers = new List<int>();
                        List<Transform> notWalls = new List<Transform>();
                        foreach (Transform i in tests)
                        {
                            if (i.gameObject.layer != LayerMask.NameToLayer("Player") && i.gameObject.layer != LayerMask.NameToLayer("LaserTarget") && i.gameObject.tag != "DontCut")
                            {
                                layers.Add(i.gameObject.layer);
                                i.gameObject.layer = LayerMask.NameToLayer("TestLayer");
                            }
                            else notWalls.Add(i);
                        }

                        print(tests.Count);
                        foreach (Transform i in notWalls) tests.Remove(i);
                        print(tests.Count);
                        Vector2 delta = end - start;
                        hits = Physics2D.RaycastAll(start, delta.normalized, delta.magnitude, paperMask);
                        for (int i = 0; i < layers.Count; i++) tests[i].gameObject.layer = layers[i];
                        if (hits.Length > 0)
                        {
                            foreach (RaycastHit2D hit in hits)
                            {
                                tests.Remove(hit.collider.transform);
                                Cut(hit.collider.gameObject, start, end, split.transform);
                            }
                        }
                        Vector2 line = CalcLine(start, end, current.transform);

                        foreach (Transform i in tests) {
                            if (i.gameObject.layer != LayerMask.NameToLayer("Player") && i.gameObject.layer != LayerMask.NameToLayer("LaserTarget") && i.gameObject.tag != "DontCut")
                            {
                                Vector2 center = CenterList(i.GetComponent<PolygonCollider2D>().points.ToList());
                                center = i.TransformPoint(center);
                                center = current.transform.InverseTransformPoint(center);
                                if (line.y + line.x * center.x > center.y)
                                {
                                    i.parent = split.transform;
                                }
                                else i.parent = current.transform;
                            }
                        }
                        UIManager.main.cutsRemaining--;
                        uiButton.GetComponentInChildren<TextMeshProUGUI>().text = "x" + UIManager.main.cutsRemaining.ToString();
                        if (UIManager.main.cutsRemaining == 0) uiButton.interactable = false;
                    }
                }
                current = null;

            }
            if (Input.GetButton("Fire1"))
            {
                endImg.transform.position = Input.mousePosition;
                Vector2 camStart, camEnd;
                camStart = startImg.transform.position;
                camEnd = Input.mousePosition;
                Vector2 delta = camEnd - camStart;
                if (delta.magnitude > lineThreshold * (lineDots.Count + 1))
                {
                    lineDots.Add(Instantiate(lineDot, lineDot.transform.parent).GetComponent<Image>());
                    lineDots[lineDots.Count - 1].gameObject.SetActive(true);
                }
                else if (delta.magnitude < lineThreshold * (lineDots.Count))
                {
                    Destroy(lineDots[0]);
                    lineDots.RemoveAt(0);
                }
                for (int i = 0; i < lineDots.Count; i++)
                {
                    lineDots[i].transform.up = delta;
                    lineDots[i].transform.position = startImg.transform.position + (Vector3)delta.normalized * lineThreshold * (i + 0.5f);
                }
            }
        }
    }

    List<Transform> GetChildrenRecursively(Transform parent) {
        List<Transform> output = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++) {
            
            output.AddRange(GetChildrenRecursively(parent.GetChild(i)));
            output.Add(parent.GetChild(i));
        }
        return output;
    }
    Vector2 CalcLine(Vector2 a, Vector2 b, Transform target = null) {
        if (target != null) {
            a = target.InverseTransformPoint(a);
            b = target.InverseTransformPoint(b);
        }
        Vector2 line;
        line.x = (b.y - a.y) / (b.x - a.x);
        line.y = b.y - (b.x * line.x);
        return line;
    }

    Vector2 CenterList(List<Vector2> input) {
        Vector2 output = Vector2.zero;
        foreach (Vector2 i in input) output += i;
        return output /= input.Count;
    }

    List<int> CircularisePoints(List<Vector2> input) {
        List<int> output = new List<int>();
        List<Polar> polars = new List<Polar>();
        Vector2 center = CenterList(input);

        for (int i = 0; i < input.Count; i++)
        {
            Polar point;
            Vector2 delta = input[i] - center;
            point.rot = Mathf.Atan2(delta.y, delta.x);
            point.siz = delta.magnitude;
            point.ind = i;
            polars.Add(point);
        }

        polars.Sort((a, b) => b.rot.CompareTo(a.rot));
        foreach (Polar i in polars) {
            output.Add(i.ind);
        }
        return output;
    }


    struct Polar {
        public float rot;
        public float siz;
        public int ind;
    }
    List<int> Triangulate(List<Vector2> input) {
        List<int> output = new List<int>();
        List<int> order = CircularisePoints(input);

        for (int i = 0; i < input.Count - 2; i++) {
            output.Add(order[0]);
            output.Add(order[i + 1]);
            output.Add(order[i + 2]);
        }

        return output;
    }

    List<Vector3> v2Tov3(List<Vector2> input) {
        List<Vector3> output = new List<Vector3>();
        foreach (Vector2 i in input) {
            output.Add(i);
        }
        return output;
    }

    GameObject Cut(GameObject target, Vector2 lineStart, Vector2 lineEnd, Transform parent = null)
    {
        Mesh mesh = target.GetComponent<MeshFilter>().mesh;
        Vector2 line;
        List<Vector2> above = new List<Vector2>();
        List<Vector2> below = new List<Vector2>();
        Vector2 delta = lineEnd - lineStart;
        int layer = target.layer;
        target.layer = LayerMask.NameToLayer("TestLayer");

        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(lineStart, delta.normalized, delta.magnitude, paperMask))
        {
            Vector2 point = target.transform.InverseTransformPoint(hit.point);
            above.Add(point);
            below.Add(point);
        }
        else
        {
            target.layer = LayerMask.NameToLayer("Paper");
            return null;
        }
        if (hit = Physics2D.Raycast(lineEnd, -delta.normalized, delta.magnitude, paperMask))
        {
            Vector2 point = target.transform.InverseTransformPoint(hit.point);
            above.Add(point);
            below.Add(point);
        }
        else
        {
            target.layer = LayerMask.NameToLayer("Paper");
            return null;
        }

        line = CalcLine(lineStart, lineEnd, target.transform);

        foreach (Vector3 i in mesh.vertices)
        {
            if (i.x * line.x + line.y > i.y) below.Add(i);
            else above.Add(i);
        }


        ApplyCut(target, above);
        GameObject split = Instantiate(target, parent);
        Rigidbody2D rb;
        if (split.TryGetComponent(out rb)) rb.AddForce((CenterList(below) - CenterList(above)).normalized*30);
        ApplyCut(split, below);

        target.layer = layer;
        split.layer = layer;
        return split;
    }

    void ApplyCut(GameObject obj, List<Vector2> points, bool linerenderer = false) {
        Mesh mesh = new Mesh();
        List<int> tris = Triangulate(points);
        mesh.vertices = v2Tov3(points).ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        obj.GetComponent<MeshFilter>().mesh = mesh;

        List<int> order = CircularisePoints(points);
        List<Vector2> polyPoints = new List<Vector2>();
        foreach (int i in order)
        {
            polyPoints.Add(points[i]);
        }
        obj.GetComponent<PolygonCollider2D>().points = polyPoints.ToArray();
        LineRenderer lr;
        if (obj.TryGetComponent(out lr))
        {
            lr.positionCount = polyPoints.Count;
            lr.SetPositions(v2Tov3(polyPoints).ToArray());
            lr.enabled = false;
        }

        /*Mesh particleMesh = new Mesh();
        particleMesh.vertices = v2Tov3(points).ToArray();
        particleMesh.triangles = order.ToArray();

        ParticleSystem.ShapeModule shape = obj.GetComponent<ParticleSystem>().shape;
        shape.mesh = particleMesh;*/
    }

}
