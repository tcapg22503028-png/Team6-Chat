using System;
using System.Data;
using UnityEngine;
using MySqlConnector;
using UnityEngine.UI;

public class uta_ChatManager : MonoBehaviour
{
    private string connStr = "server=172.16.2.26;user id=tateno;password=ae21215926;database=uta";
    public InputField inputField;
    public Text chatText;

    private float sendInterval = 0.5f;
    private float sendTimer = 0f;

    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        chatText.text += inputField.text.TrimEnd('\n') + "\n";
        inputField.text = "";
        inputField.ActivateInputField(); // 入力欄にフォーカスを戻す
    }

    void Start()
    {

    }

    void Update()
    {
        if (inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }

        if (sendTimer >= sendInterval)
        {
            SendPositionToDB();
            sendTimer = 0f;
        }
    }

    private void SendPositionToDB()
    {
        float posX = transform.position.x;
        float posY = transform.position.y;
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string slq = "update pos set x=" + posX + ",y=" + posY + " where uta =1";
                MySqlCommand cmd = new MySqlCommand(slq, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
        }
    }
}
