using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class ScoreManager : MonoBehaviour {

    public GameObject loadingPrefab;
    public GameObject scoreBoard;
    public Button button;

    string value;
    long flag;

    Thread thread;
    Dictionary<string, Dictionary<string, int>> userScores;

    void Update()
    {
        if (thread != null && !thread.IsAlive)
        {
            thread = null;
            if (Interlocked.Read(ref flag) > 0)
            {
                SetScore(value, "score", (int)GameObject.FindObjectOfType<Hovercraft>().pedestriansCount);
                SetScore(value, "time", (int)GameObject.FindObjectOfType<Hovercraft>().time);
            }
            button.transform.parent.gameObject.SetActive(false);
            loadingPrefab.SetActive(false);
            scoreBoard.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    public int GetScore(string username, string scoreType)
    {
        if (userScores == null) userScores = new Dictionary<string, Dictionary<string, int>>();

        if (!userScores.ContainsKey(username))
        {
            return 0;
        }
        if (!userScores[username].ContainsKey(scoreType))
        {
            return 0;
        }
        return userScores[username][scoreType];
    }

    public void SetScore(string username, string scoreType, int value)
    {
        if (userScores == null) userScores = new Dictionary<string, Dictionary<string, int>>();
        if (!userScores.ContainsKey(username))
        {
            userScores[username] = new Dictionary<string, int>();
        }
        userScores[username][scoreType] = value;
    }

    public string[] GetUserNames(string sortingType)
    {
        if (userScores == null) userScores = new Dictionary<string, Dictionary<string, int>>();

        return userScores.Keys.OrderByDescending(n => GetScore(n, sortingType)).ToArray();
    }

    public void ToggleButton(string value)
    {
        this.value = value;
        if (string.IsNullOrEmpty(value))
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void LoadScene(int scene)
    {
        Application.LoadLevel(scene);
    }

    void LoadFromDatabase(string pname, int pscore, int ptime)
    {
        MySqlConnection con = null;
        MySqlCommand cmd = null;
        MySqlDataReader rdr = null;
        string query = string.Empty;

        try
        {
            con = new MySqlConnection("Server=www.db4free.net;Database=ld33;User ID=ld33;Password=ludumdare;Pooling=true");
            con.Open();
            Debug.Log("Connection State: " + con.State);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            flag = 1;
            goto Finish;
        }

        try
        {
            query = "INSERT INTO `scores` (`name`, `score`, `time`) VALUES (?Name, ?Score, ?Time)";
            if (con.State.ToString() != "Open")
                con.Open();

            cmd = new MySqlCommand(query, con);
            MySqlParameter nameParam = cmd.Parameters.Add("?Name", MySqlDbType.VarChar);
            nameParam.Value = pname;
            MySqlParameter scoreParam = cmd.Parameters.Add("?Score", MySqlDbType.Int16);
            scoreParam.Value = pscore;
            MySqlParameter timeParam = cmd.Parameters.Add("?Time", MySqlDbType.Int16);
            timeParam.Value = ptime;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (con != null)
            {
                if (con.State.ToString() != "Closed")
                    con.Close();
            }
            Debug.LogError(ex.ToString());
            flag = 1;
            goto Finish;
        }

        try
        {
            query = "SELECT * FROM `scores`";
            if (con.State.ToString() != "Open")
                con.Open();

            cmd = new MySqlCommand(query, con);
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    string name = rdr["name"].ToString();
                    int score = int.Parse(rdr["score"].ToString());
                    int time = int.Parse(rdr["time"].ToString());
                    SetScore(name, "score", score);
                    SetScore(name, "time", time);
                }
                rdr.Close();
            }

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                if (con.State.ToString() != "Closed")
                    con.Close();
            }
            Debug.LogError(ex.ToString());
            Interlocked.Increment(ref flag);
            goto Finish;
        }

        if (con != null)
        {
            if (con.State.ToString() != "Closed")
                con.Close();
        }

        Finish:
        {
            return;
        }
    }

    public void SubmitScore()
    {
        button.transform.parent.gameObject.SetActive(false);
        loadingPrefab.SetActive(true);
        int score = (int)GameObject.FindObjectOfType<Hovercraft>().pedestriansCount;
        int time = (int)GameObject.FindObjectOfType<Hovercraft>().time;
        thread = new Thread(
            () => LoadFromDatabase(value, score, time));
        thread.Start();
    }
}
