using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawConnection : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject src;
    public GameObject dest;
    public Material colour;

    private float zValue = -0.001f;

    void Start()
    {
        LineRenderer connection = gameObject.GetComponent<LineRenderer>();

        List<Vector3> pos = new List<Vector3>();
        connection.material = colour;
        pos.Add(new Vector3(src.transform.position.x, src.transform.position.y, zValue));
        pos.Add(new Vector3(dest.transform.position.x, dest.transform.position.y, zValue));
        connection.startWidth = 0.2f;
        connection.endWidth = 0.2f;
        connection.SetPositions(pos.ToArray());
        connection.useWorldSpace = true;
        


    }
}
