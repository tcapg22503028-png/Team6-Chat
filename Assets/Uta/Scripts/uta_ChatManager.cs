using MySqlConnector;
using System;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
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
    private int userId; // 自分のユーザーID

    private float sendTimer = 0f;
    private float chatTimer = 0f;
    private int lastMessageId = 0;

    public ScrollRect scrollRect;

    public Transform content;           // ScrollView/Content
    public GameObject messagePrefab;    // ChatMessage プレハブ

    public uta_ConfirmDialogManager confirmDialog;

    void Start()
    {
        if (PlayerPrefs.HasKey("UserId"))
        {
            userId = PlayerPrefs.GetInt("UserId");
        }
        else
        {
            userId = CreateNewUser();
            PlayerPrefs.SetInt("UserId", userId);
            PlayerPrefs.Save();
        }

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

    int CreateNewUser()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "INSERT INTO Users () VALUES (); SELECT LAST_INSERT_ID();";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    string GetUserColor(int userId)
    {
        switch (userId)
        {
            case 1: return "#FF0000"; // 赤
            case 2: return "#0000ff"; // 青
            case 3: return "#00ff00"; // 緑
            case 4: return "#ffff00"; // 黄
            case 5: return "#ffa500"; // オレンジ
            case 6: return "#9400d3"; // 紫
            case 7: return "#ee82ee"; // ピンク
            case 8: return "#00ffff"; // 水色
            default: return "#000000"; // 9以降は黒
        }
    }

    public void ShowConfirmDialog(int messageId)
    {
        confirmDialog.Setup("このメッセージを削除しますか？", messageId, this);
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
                        bool added = false;

                        while (reader.Read())
                        {
                            added = true;

                            int id = reader.GetInt32(0);
                            int uid = reader.GetInt32(1);
                            string msg = reader.GetString(2);

                            string color = GetUserColor(uid);
                            string text;

                            //chatText.text += $"<color={color}>User {uid}:</color> {msg}\n";
                            if (uid == userId)
                            {
                                // 自分のメッセージ
                                text = chatText.text += $"<b><color={color}>User {uid}:</color></b> {msg}\n";
                            }
                            else
                            {
                                // 他人のメッセージ
                                text = chatText.text += $"<color={color}>User {uid}:</color> {msg}\n";
                            }

                            GameObject obj = Instantiate(messagePrefab, content);
                            var ui = obj.GetComponent<uta_ChatMessageUI>();
                            ui.Setup(id, text, this);

                            lastMessageId = id;
                        }

                        // 新しいメッセージがあったら一番下へ
                        if (added)
                        {
                            Canvas.ForceUpdateCanvases();
                            scrollRect.verticalNormalizedPosition = 0f;
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