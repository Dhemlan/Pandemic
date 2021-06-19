using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDiseaseCubeRotator : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0,0, 100) * Time.deltaTime);
    }
}
