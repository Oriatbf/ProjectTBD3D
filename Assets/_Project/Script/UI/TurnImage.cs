using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnImage : MonoBehaviour
{
    [SerializeField]private Image image;
    [SerializeField]private TextMeshProUGUI text;

    public void SetInfo(string info)
    {
        text.text = info;
    }
}
