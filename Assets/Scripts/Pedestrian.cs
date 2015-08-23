using UnityEngine;
using System.Collections;

public class Pedestrian : MonoBehaviour {

    public AudioClip[] hurt;
    public Ragdoll root;
    public GameObject WalkBoundaries;
    public float Speed;
    public float SmoothTime;

    Bounds walkBoundaries;
    Vector3 velocity, direction, lastDirection; // ha!
    bool triggered;
    float lastRot;

    void Start()
    {
        velocity = Vector3.zero;
        direction = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up) * Vector3.right;
        direction *= Speed;
        lastDirection = direction;
        triggered = false;

        Vector3 newSize = WalkBoundaries.GetComponent<Renderer>().bounds.size;
        newSize.y = 10;
        walkBoundaries = new Bounds(WalkBoundaries.GetComponent<Renderer>().bounds.center, newSize);
    }

    void Update()
    {
        if (triggered) { return; }
        if (!walkBoundaries.Contains(transform.position)) Destroy(gameObject);
        if(/**!walkBoundaries.Contains(transform.position + direction*10.0f) || */(Time.time - lastRot) >= 5 && Random.value >= 0.5f) {
            direction = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up) * Vector3.right;
            direction *= Speed;
            lastRot = Time.time;
        }

        Debug.DrawLine(transform.position, transform.position + -transform.right * 2.5f, Color.red);
        RaycastHit hit;
        if (Physics.Linecast(transform.position, transform.position + -transform.right * 2.5f, out hit))
        {
            if (hit.distance < 2.5f)
            {
                direction = hit.normal * Speed;
            }
        }

        lastDirection = Vector3.SmoothDamp(lastDirection, direction, ref velocity, SmoothTime);
        transform.position = transform.position + lastDirection;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lastDirection, Vector3.up), 10.0f * Time.deltaTime);
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.y+45, 0);
        //Vector3 vel_n = direction.normalized;
        //Debug.DrawLine(pedestrian.position, pedestrian.position + vel_n, Color.red, 0, true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) { return; }

        (other.GetComponent<Hovercraft>() as Hovercraft).OnPedestrianKill();
        GetComponent<AudioSource>().clip = hurt[Random.Range(0, hurt.Length)];
        GetComponent<AudioSource>().Play();
        triggered = true;
        gameObject.tag = "Dead";
        Destroy(gameObject, 15);
        Destroy(GetComponent<Animator>());
        root.InstantiateRagdoll(null);
    }
}
