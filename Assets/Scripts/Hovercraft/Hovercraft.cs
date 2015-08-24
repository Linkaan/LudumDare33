using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hovercraft : MonoBehaviour {

    public Ragdoll root;
    public Text PedestriansCountText;
    public Text ProgressText;
    public RectTransform Progress;
    public Image VisualProgress;

    public GameObject ScoreboardInputDialogue;
    public GameObject HUD;

    public Color MaxColor;
    public Color MinColor;

    public float Acceleration;
    public float RotationRate;

    public float TurnRotationAngle;
    public float TurnRotationSpeed;

    public float difficulty;
    public float maxBloodlust;

    [HideInInspector]
    public float pedestriansCount;
    [HideInInspector]
    public float time;

    float cachedY;
    float maxXValue;
    float minXValue;
    float currentBloodlust;

    float startTime;
    float rotVel;
    bool triggered;

    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        triggered = false;
        cachedY = Progress.position.y;
        maxXValue = Progress.position.x;
        minXValue = Progress.position.x - Progress.rect.width;
        currentBloodlust = maxBloodlust;

        startTime = Time.time;
    }

    void Update()
    {
        if (triggered) { return; };

        if ((currentBloodlust -= Time.deltaTime * (Mathf.Min(pedestriansCount / 10.0f, difficulty * 1.5f))) <= 0.0f) Lose();
        ProgressText.text = "Bloodlust: " + Mathf.RoundToInt(currentBloodlust);

        float currentXValue = MapValues(currentBloodlust, 0, maxBloodlust, minXValue, maxXValue);
        Progress.position = new Vector3(currentXValue, cachedY);

        VisualProgress.color = Color.Lerp(MinColor, MaxColor, currentBloodlust / maxBloodlust);
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
            Lose();
            rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            root.InstantiateRagdoll(null);
            (root.GetComponent<Rigidbody>() as Rigidbody).AddExplosionForce(col.relativeVelocity.magnitude*35f, root.transform.position, 5f);
        }
    }

    void Lose()
    {
        triggered = true;
        time = Time.time - startTime;
        HUD.SetActive(false);
        ScoreboardInputDialogue.SetActive(true);
    }

    public void OnPedestrianKill()
    {
        if (triggered) { return; };

        PedestriansCountText.text = string.Format("{0}", ++pedestriansCount);
        if((currentBloodlust += Mathf.Max(difficulty / (pedestriansCount / 10.0f), 2.5f)) > maxBloodlust) {
            currentBloodlust = maxBloodlust;
        }  
    }

    float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
