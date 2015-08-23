using UnityEngine;
using System.Collections;

public class ProceduralTerrainGeneration : MonoBehaviour {

    public GameObject SpawnBoundaries;

    public GameObject[] HousePrefabs;
    public int AmountOfHouses;

    Bounds spawnBoundaries;

    void Awake()
    {
        Vector3 newSize = SpawnBoundaries.GetComponent<Renderer>().bounds.size;
        spawnBoundaries = new Bounds(SpawnBoundaries.GetComponent<Renderer>().bounds.center, newSize);

        float sqrtAmount = (int)Mathf.Sqrt(AmountOfHouses);

        int xInc = (int)(spawnBoundaries.size.x / sqrtAmount);
        int zInc = (int)(spawnBoundaries.size.z / sqrtAmount);


        for (int x = 0; x < sqrtAmount; x++)
        {
            for (int z = 0; z < sqrtAmount; z++)
            {
                GameObject housePrefab = HousePrefabs[Random.Range(0, HousePrefabs.Length-1)];
                float width = housePrefab.GetComponent<Renderer>().bounds.size.x;
                float depth = housePrefab.GetComponent<Renderer>().bounds.size.z;

                Vector3 position = new Vector3((xInc) * x - spawnBoundaries.center.x - spawnBoundaries.size.x/2 + width, 0, (zInc) * z - spawnBoundaries.center.z - spawnBoundaries.size.z/2 + depth);
                Instantiate(housePrefab, position, Quaternion.Euler(new Vector3(-90, 90 * Random.Range(0, 3), 0)));
            }
        }
    }
}
