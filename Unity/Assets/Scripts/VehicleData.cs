using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VehicleData
{
    public List<float> targetPosition;
    public List<float> targetRotation;
    public float targetForkPosition;

    public VehicleData()
    {
        targetPosition = new List<float>(new float[3]);
        targetRotation = new List<float>(new float[3]);
    }
    public void Set(Vector3 p, Vector3 r, float f)
    {
        targetPosition[0] = p.x;
        targetPosition[1] = p.y;
        targetPosition[2] = p.z;
        targetRotation[0] = r.x;
        targetRotation[1] = r.y;
        targetRotation[2] = r.z;
        targetForkPosition = f;
    }
}
