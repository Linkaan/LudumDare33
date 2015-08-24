using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserscoreList : MonoBehaviour {

    public GameObject UserscoreEntryPrefab;

    ScoreManager scoreManager;

    public void UpdateScoreboard()
    {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }

        string[] userids = scoreManager.GetUserIDs("score");

        foreach (string userid in userids)
        {
            GameObject go = Instantiate(UserscoreEntryPrefab) as GameObject;
            go.transform.SetParent(this.transform);
            go.transform.Find("Username").GetComponent<Text>().text = scoreManager.GetUserName(userid);
            go.transform.Find("Highscore").GetComponent<Text>().text = scoreManager.GetScore(userid, "score").ToString();
            go.transform.Find("Time").GetComponent<Text>().text = scoreManager.GetScore(userid, "time").ToString();
        }
    }
}
