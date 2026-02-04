using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class uta_ChatMessageUI : MonoBehaviour, IPointerClickHandler
{
    public Text messageText;

    private int messageId;
    private uta_ChatManager chatManager;

    public void Setup(int msgId, string text, uta_ChatManager manager)
    {
        messageId = msgId;
        messageText.text = text;
        chatManager = manager;
    }


    // 右クリックのみ
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            chatManager.ShowConfirmDialog(messageId, this);
        }
    }

    public void DeleteSelf()
    {
        Destroy(gameObject);
    }
}
