using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerTurnEnd : MonoBehaviour
{
    [SerializeField] Button turnEndBtn, nextStageBtn;

    private void Start()
    {
        nextStageBtn.gameObject.SetActive(false);
        turnEndBtn.gameObject.SetActive(true);
    }

    public void SetTurnEndAction( Action action )
    {
        if (action != null)
        {
            turnEndBtn.onClick.RemoveAllListeners();
            turnEndBtn.onClick.AddListener(action.Invoke);
        }
       
    }

    public void SetNextStageAction(Action action)
    {
        if (action != null)
        {
            nextStageBtn.onClick.RemoveAllListeners();
            nextStageBtn.onClick.AddListener(action.Invoke);
        }
    }

    public void NextStageActive()
    {
        nextStageBtn.gameObject.SetActive(true);
        turnEndBtn.gameObject.SetActive(false);
    }
}
