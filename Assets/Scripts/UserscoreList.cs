using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserscoreList : MonoBehaviour {

    public GameObject UserscoreEntryPrefab;

    ScoreManager scoreManager;

    void Start()
    {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        string[] names = scoreManager.GetUserNames("score");

        foreach(string name in names)
        {
            GameObject go = Instantiate(UserscoreEntryPrefab) as GameObject;
            go.transform.SetParent(this.transform);
            go.transform.Find("Username").GetComponent<Text>().text = name;
            go.transform.Find("Highscore").GetComponent<Text>().text = scoreManager.GetScore(name, "score").ToString();
            go.transform.Find("Time").GetComponent<Text>().text = scoreManager.GetScore(name, "time").ToString();
        }
    }
}
