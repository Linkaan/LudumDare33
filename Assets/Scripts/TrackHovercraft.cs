using UnityEngine;
using System.Collections;

public class TrackHovercraft : MonoBehaviour {

    public Transform Target;
    public float DistanceUp;
    public float DistanceBack;
    public float LookForward;
    public float MinimumHeight;

    Vector3 posVel;

    void FixedUpdate()
    {
        if (Target == null) { return; }

        Vector3 newPos = Target.position + (Target.forward * -DistanceBack);
        newPos.y = Mathf.Max(newPos.y + DistanceUp, MinimumHeight);

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref posVel, 0.20f);

        Vector3 focalPoint = Target.position + (Target.forward * LookForward);
        transform.LookAt(focalPoint);
    }
}
