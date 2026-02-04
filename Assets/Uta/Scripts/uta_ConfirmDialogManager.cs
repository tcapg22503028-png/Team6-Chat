using UnityEngine;
using UnityEngine.UI;

public class uta_ConfirmDialogManager : MonoBehaviour
{
    public GameObject panel;
    public Text messageText;
    public Button yesButton;
    public Button noButton;

    private int targetMessageId;
    private uta_ChatManager chatManager;

    public void Setup(string msg, int messageId, uta_ChatManager manager)
    {
        targetMessageId = messageId;
        messageText.text = msg;
        chatManager = manager;

        panel.SetActive(true);
    }

    public void OnYes()
    {
        chatManager.DeleteMessage(targetMessageId);
        panel.SetActive(false);
    }

    public void OnNo()
    {
        panel.SetActive(false);
    }
}
