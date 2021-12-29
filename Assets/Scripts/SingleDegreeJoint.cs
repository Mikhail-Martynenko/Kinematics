using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDegreeJoint : MonoBehaviour
{
    public enum JointDegree
    {
        RotateX = 0,
        RotateY = 1,
        RotateZ = 2
    }
    public JointDegree degreeOfFreedom;
    public Vector3 _axis;

    public float MinAngle;
    public float MaxAngle;

    public Vector3 StartOffset;
    void Start()
    {
        _axis = degreeOfFreedom switch
        {
            JointDegree.RotateX => new Vector3(1, 0, 0),
            JointDegree.RotateY => new Vector3(0, 1, 0),
            JointDegree.RotateZ => new Vector3(0, 0, 1),
            _ => _axis
        };

        StartOffset = transform.localPosition * 2;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void SetValue(float value)
    {
        transform.localEulerAngles = degreeOfFreedom switch
        {
            JointDegree.RotateX => new Vector3(value, 0, 0),
            JointDegree.RotateY => new Vector3(0, value, 0),
            JointDegree.RotateZ => new Vector3(0, 0, value),
            _ => transform.localEulerAngles
        };
    }

    public float GetValue()
    {
        return transform.rotation.eulerAngles[(int) degreeOfFreedom];
    }
    
}

