using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Agent
{
    // Fields for Seeker
    [SerializeField, Range(0, 1)] float cohesionScalar = 1f;
    [SerializeField, Range(0, 1)] float boundsStrength = 1f;

    private ChildTracker childTracker;

    private void Start()
    {
        childTracker = GetComponentInParent<ChildTracker>();
    }

    // Calculate the steering force
    protected override Vector3 CalcSteering()
    {
        return Cohere(childTracker.AveragePosition) * cohesionScalar +
            StayInBoundsForce() * boundsStrength;
    }
}
