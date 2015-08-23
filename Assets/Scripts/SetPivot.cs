using UnityEngine;
using System.Collections;

public class SetPivot : MonoBehaviour {

    public GameObject AnchorOne;
    public GameObject AnchorTwo;

    void Awake()
    {
        this.transform.position = 0.5f * (AnchorOne.transform.position + AnchorTwo.transform.position);
    }
}
