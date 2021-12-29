using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyDegreeJoints : MonoBehaviour
{
    public float MinAngle;
    public float MaxAngle;

    public Vector3 StartOffset;
    void Start()
    {
        StartOffset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetValue(float rotX, float rotZ)
    {
        transform.eulerAngles = new Vector3(rotX, 0, rotZ);
    }

    public float GetXValue()
    {
        return transform.rotation.eulerAngles[0];

    }

    public float GetZValue()
    {
        return transform.rotation.eulerAngles[2];
    }
}


