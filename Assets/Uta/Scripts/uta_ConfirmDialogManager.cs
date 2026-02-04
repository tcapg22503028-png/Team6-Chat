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

    private uta_ChatMessageUI targetUI;

    public void Setup(string msg, int messageId, uta_ChatMessageUI ui, uta_ChatManager manager)
    {
        targetMessageId = messageId;
        targetUI = ui;
        messageText.text = msg;
        chatManager = manager;
        panel.SetActive(true);
    }

    public void OnYes()
    {
        chatManager.DeleteMessage(targetMessageId);
        targetUI.DeleteSelf();   // UI�𑦍폜
        panel.SetActive(false);
    }

    public void OnNo()
    {
        panel.SetActive(false);
    }
}
