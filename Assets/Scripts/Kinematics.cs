using UnityEngine;


public class Kinematics : MonoBehaviour
{
    private SingleDegreeJoint[] joints;
    public float speed = 2;
    public float[] solution;

    //public Vector3 position;
    public GameObject target;
    //public float distance;
    public float[] cos;
    public Vector3[] points = new Vector3[3];

    //delta x
    float SamplingDistance = 1f;

    float LearningRate = 10f;

    float DistanceThreshold = 0.2f;

    Vector3 actorLength = new Vector3(0,1,0);

    // Start is called before the first frame update
    void Start()
    {
        joints = GetComponentsInChildren<SingleDegreeJoint>();
        solution = new float[joints.Length];
        cos = new float[joints.Length-1];
        for (var i = 0; i < joints.Length; i++)
        {
            //distance = Vector3.Distance(position, target.transform.position);
            solution[i] = joints[i].GetValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Solve();
        
        solution = InverseKinematics(target.transform.position, solution);

        Debug.Log(solution[0]);

        for (int i = 0; i < joints.Length; i++)
            joints[i].SetValue(solution[i]);
    }

    [ContextMenu("DrawForward")]
    public void DrawForward()
    {
        Instantiate(target, ForwardKinematics(solution), Quaternion.identity);

    }
    /*
    private void Solve()
    {
        var delta = Time.deltaTime * speed;
        /*
        for (var j = 0; j < solution.Length; j++)
            solution[j] = InRange(solution[j] + (j + 1) * delta);

        //
        
        /*
        for (var i = 0; i < cos.Length; i++)
        {
            points[0] = joints[i].transform.position;
            points[1] = joints[i+1].transform.position;
            points[2] = target.transform.position;
            cos[i] = angle_point(points[0], points[2], points[1]);
            if (cos[i] >= 1e-6)
                solution[i] = InRange(solution[i] + (i + 1) * delta);
        }//

    }

    private float InRange(float value)
    {
        while (value < 0)
        {
            value += 360;
        }

        while (value > 360)
        {
            value -= 360;
        }

        return value;

        
    }

    private float angle_point(Vector3 a, Vector3 b, Vector3 c)
    {
        float x1 = a.x - b.x, x2 = c.x - b.x;
        float y1 = a.y - b.y, y2 = c.y - b.y;
        float z1 = a.z - b.z, z2 = c.z - b.z;
        float d1 = Mathf.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
        float d2 = Mathf.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);
        return Mathf.Acos((x1 * x2 + y1 * y2 + z1 * z2) / (d1 * d2));
    }
    //*/
    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevPoint = joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        Vector3 nextPoint;
        for (int i = 1; i < joints.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i-1], joints[i-1]._axis);
            nextPoint = prevPoint + rotation * joints[i].StartOffset;

            prevPoint = nextPoint;
        }

        rotation *= Quaternion.AngleAxis(angles[joints.Length-1], joints[joints.Length - 1]._axis);
        nextPoint = prevPoint + rotation * (joints[joints.Length-1].StartOffset+actorLength);

        prevPoint = nextPoint;

        return prevPoint;
    }

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }


    //ÍÀÄÎ ÐÀÇÎÁÐÀÒÜÑß Ñ ÏÅÐÅÄÀ×ÅÉ ÌÀÑÑÈÂÀ
    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);
        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);
        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;
        return gradient;
    }

    public float[] InverseKinematics(Vector3 target, float[] angles)
    {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
            return angles;
        for (int i = joints.Length - 1; i >= 0; i--)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            //float[] partialGradient = PartialGradient(target, angles, i);
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;

            // Clamp
            angles[i] = Mathf.Clamp(angles[i], joints[i].MinAngle, joints[i].MaxAngle);

            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return angles;
        }

        return angles;
    }


}
