using TMPro;
using UnityEngine;

public class BuffInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoTxt;
    [SerializeField] private RectTransform infoUI;
    public void InitData(string info, Vector3 targetPos)
    {
        infoTxt.text = info;
        SetPos(targetPos);
    }
    
    private void SetPos(Vector3 targetPos)
    {
        if (targetPos.y <= 400) infoUI.pivot = Vector2.zero;
        else infoUI.pivot = new Vector2(0,1);
        infoUI.position = targetPos;
    }
}
