using UnityEngine;

public class Fleer : Agent
{
    // Fields for Seeker
    [SerializeField] GameObject target;
    [SerializeField, Range(0, 1)] float fleeScalar;
    [SerializeField] float fleeRadius = 6f;
    [SerializeField, Range(0, 1)] float boundsStrength = 1f;

    // Calculate the steering force
    protected override Vector3 CalcSteering()
    {
        return Flee(target, fleeRadius) * fleeScalar +
                StayInBoundsForce() * boundsStrength;
    }

}
