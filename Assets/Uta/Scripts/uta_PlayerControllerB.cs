using System;
using System.Data;
using UnityEngine;
using MySqlConnector;

public class uta_PlayerControllerB : MonoBehaviour
{
    private string connStr = "server=172.16.2.26;user id=tateno;password=ae21215926;database=uta";
    private float fetchInterval = 0.5f;
    private float fetchTimer = 0f;

    void Start()
    {

    }

    void Update()
    {
        fetchTimer += Time.deltaTime;
        if (fetchTimer >= fetchInterval)
        {
            LoadPos();
            fetchTimer = 0f;
        }
    }

    private void LoadPos()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "select x,y from pos where uta=1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        float x = Convert.ToSingle(reader["x"]);
                        float y = Convert.ToSingle(reader["y"]);
                        transform.position = new Vector2(x, y);
                    }
                }
            }
            catch
            {

            }
        }
    }
}