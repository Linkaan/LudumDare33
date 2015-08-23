using UnityEngine;
using System.Collections;

public class PedestrianSpawner : MonoBehaviour {

    public LayerMask mask;
    public Camera Camera;
    public GameObject Prefab;
    public GameObject SpawnBoundaries;
    public float MaxPedestrianCount;

    float pedestrianCount = 0;
    float frames;
	
	void Update () {
        if (++frames % 30 == 0) pedestrianCount = GameObject.FindGameObjectsWithTag("Pedestrian").Length;
        if (pedestrianCount < MaxPedestrianCount)
        {
            //if ((Time.time - startTime) < 3f) return;
            float randX = Random.Range(SpawnBoundaries.transform.position.x - SpawnBoundaries.transform.localScale.x / 2, SpawnBoundaries.transform.position.x + SpawnBoundaries.transform.localScale.x / 2);
            float randZ = Random.Range(SpawnBoundaries.transform.position.z - SpawnBoundaries.transform.localScale.z / 2, SpawnBoundaries.transform.position.z + SpawnBoundaries.transform.localScale.z / 2);
            Vector3 randPos = new Vector3(randX, 0.75f, randZ);
            if (Physics.OverlapSphere(randPos, 1f, mask).Length > 0) return;
            Bounds randBounds = new Bounds(randPos, (Prefab.GetComponent<BoxCollider>().bounds.size));
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera);
            if (GeometryUtility.TestPlanesAABB(planes, randBounds))
            {
                RaycastHit hit;
                Vector3 rayDir = Camera.transform.position - randPos;
                if (Physics.Raycast(randPos, rayDir, out hit))
                {
                    if (hit.transform != Camera)
                    {
                        Instantiate(Prefab, randPos, Quaternion.Euler(new Vector3(90, 0, 0)));
                    }
                }
            }
            else
            {
                Instantiate(Prefab, randPos, Quaternion.Euler(new Vector3(90, 0, 0)));
            }
        }
	}
}
