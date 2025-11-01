using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnEnd : MonoBehaviour
{
    [SerializeField] Button btn;

    public void SetAction( Action action )
    {
        if (action != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action.Invoke);
        }
       
    }
}
