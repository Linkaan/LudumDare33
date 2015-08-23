using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour {

    public float ThrusterDistance;
    public float ThrusterStrength;
    public Transform[] thrusters;

    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 baseforce;
        float distancePercentage;
        RaycastHit hit;
        foreach (Transform thruster in thrusters)
        {
            if (Physics.Raycast(thruster.position, thruster.up * -1, out hit, ThrusterDistance))
            {
                distancePercentage = 1 - (hit.distance / ThrusterDistance);
                baseforce = thruster.up * ThrusterStrength * distancePercentage;
                baseforce *= Time.deltaTime * rigidbody.mass;

                rigidbody.AddForceAtPosition(baseforce, thruster.position);
            }
        }
    }
}
