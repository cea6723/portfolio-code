using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seperater : Agent
{
    // Fields for Seeker
    [SerializeField, Range(0, 1)] float seperateScalar;
    [SerializeField] float seperateRadius = 3f;
    [SerializeField, Range(0, 1)] float boundsStrength = 1f;
    [SerializeField] Color radiusColor = Color.cyan;

    private ChildTracker childTracker = new ChildTracker();

    private void Start()
    {
        childTracker = GetComponentInParent<ChildTracker>();
    }

    // Calculate the steering force
    protected override Vector3 CalcSteering()
    {
        return Seperate(childTracker.GetChildPositionsNear(transform.position, seperateRadius), transform.position) * seperateScalar +
                StayInBoundsForce() * boundsStrength;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = radiusColor;
        Gizmos.DrawSphere(transform.position, seperateRadius);
    }
}
