using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Loading : MonoBehaviour {

    public string[] loadinganim;
    public float speed;

    float delta;
    int index;

    void Start()
    {
        index = 0;
        this.GetComponent<Text>().text = loadinganim[index];
    }

    void Update()
    {
        if ((delta += Time.deltaTime) > speed)
        {
            delta = 0.0f;
            if (++index == 3) index = 0;
            this.GetComponent<Text>().text = loadinganim[index];
        }
    }
}
