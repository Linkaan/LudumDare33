using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hovercraft : MonoBehaviour {

    public Ragdoll root;
    public Text PedestriansCountText;

    public float Acceleration;
    public float RotationRate;

    public float TurnRotationAngle;
    public float TurnRotationSpeed;

    float pedestriansCount;
    float rotVel;
    bool triggered;

    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        triggered = false;
    }

    void FixedUpdate()
    {
        if (triggered) { return; };

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, 3f))
        {
            rigidbody.drag = 1;

            Vector3 baseforce = transform.forward * Acceleration * Input.GetAxis("Vertical");
            baseforce *= Time.deltaTime * rigidbody.mass;

            float lubricity = 1.0f;
            // terrain stuff
            rigidbody.AddForce(baseforce * lubricity);
        }
        else
        {
            rigidbody.drag = 0;
        }

        Vector3 torque = Vector3.up * RotationRate * Input.GetAxis("Horizontal");
        torque *= Time.deltaTime * rigidbody.mass;
        rigidbody.AddTorque(torque);

        Vector3 newRot = transform.eulerAngles;
        newRot.z = Mathf.SmoothDampAngle(newRot.z, Input.GetAxis("Horizontal") * -TurnRotationAngle, ref rotVel, TurnRotationSpeed);
        transform.eulerAngles = newRot;
    }

    void OnCollisionEnter(Collision col)
    {
        if (triggered || !col.gameObject.CompareTag("Obstacle")) { return; }

        if (col.relativeVelocity.magnitude > 15.0f)
        {
            triggered = true;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            root.InstantiateRagdoll(null);
            (root.GetComponent<Rigidbody>() as Rigidbody).AddExplosionForce(col.relativeVelocity.magnitude*35f, root.transform.position, 5f);
        }
    }

    public void OnPedestrianKill()
    {
        PedestriansCountText.text = string.Format("{0} Pedestrians Slaughtered!", ++pedestriansCount);
    }
}
