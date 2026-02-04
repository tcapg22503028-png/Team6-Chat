using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class uta_ChatMessageUI : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Text messageText;

    private int messageId;
    private uta_ChatManager chatManager;

    private float holdTime = 0.6f;   // 長押し判定時間
    private float holdTimer = 0f;
    private bool isHolding = false;

    public void Setup(int msgId, string text, uta_ChatManager manager)
    {
        messageId = msgId;
        messageText.text = text;
        chatManager = manager;
    }

    // 右クリック
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            chatManager.ShowConfirmDialog(messageId);
        }
    }

    // 長押し開始
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f;
    }

    // 長押し終了
    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdTimer = 0f;
    }

    void Update()
    {
        if (!isHolding) return;

        holdTimer += Time.deltaTime;
        if (holdTimer >= holdTime)
        {
            isHolding = false;
            holdTimer = 0f;
            chatManager.ShowConfirmDialog(messageId);
        }
    }
}
