using System;
using System.Data;
using UnityEngine;
using MySqlConnector;
using UnityEngine.UI;

public class uta_ChatManager : MonoBehaviour
{
    private string connStr = "server=172.16.2.26;user id=tateno;password=ae21215926;database=uta";
    [Header("UI")]
    public InputField inputField;
    public Text chatText;

    [Header("Settings")]
    private float sendInterval = 0.5f;
    public float chatRefreshInterval = 1f;
    public int userId = 1; // 自分のユーザーID

    private float sendTimer = 0f;
    private float chatTimer = 0f;
    private int lastMessageId = 0;

    void Start()
    {
        inputField.ActivateInputField();
    }

    void Update()
    {
        // Enterでチャット送信
        if (inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageToDB();
        }

        // 一定間隔で位置をDBに送信
        sendTimer += Time.deltaTime;
        if (sendTimer >= sendInterval)
        {
            SendPositionToDB();
            sendTimer = 0f;
        }

        // 一定間隔でチャットを取得
        chatTimer += Time.deltaTime;
        if (chatTimer >= chatRefreshInterval)
        {
            RefreshChat();
            chatTimer = 0f;
        }
    }

    #region Chat Functions
    private void SendMessageToDB()
    {
        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        using (var conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "INSERT INTO Messages (UserId, MessageText) VALUES (@uid, @msg)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@msg", text);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SendMessageToDB Error: " + e.Message);
            }
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void RefreshChat()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "SELECT MessageId, UserId, MessageText FROM Messages WHERE IsDeleted=0 AND MessageId > @lastId ORDER BY CreatedAt ASC";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lastId", lastMessageId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int uid = reader.GetInt32(1);
                            string msg = reader.GetString(2);

                            chatText.text += $"User {uid}: {msg}\n";
                            lastMessageId = id;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("RefreshChat Error: " + e.Message);
            }
        }
    }

    public void DeleteMessage(int messageId)
    {
        using (var conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "UPDATE Messages SET IsDeleted=1 WHERE MessageId=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", messageId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("DeleteMessage Error: " + e.Message);
            }
        }

        // 削除後にチャットを再取得
        lastMessageId = 0;
        chatText.text = "";
        RefreshChat();
    }
    #endregion

    #region Position Functions
    private void SendPositionToDB()
    {
        float posX = transform.position.x;
        float posY = transform.position.y;

        using (var conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "UPDATE pos SET x=@x, y=@y WHERE uta=1";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@x", posX);
                    cmd.Parameters.AddWithValue("@y", posY);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SendPositionToDB Error: " + e.Message);
            }
        }
    }
    #endregion
}