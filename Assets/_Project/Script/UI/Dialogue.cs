using System;
using _Project.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour,IPoolable
{
    [SerializeField] private TextMeshProUGUI dialogueTxt;
    [SerializeField] private Button nextBtn,acceptBtn,rejectBtn;
    private EventDialogueSO _eventDialogueSO;
    private int _curIndex = 0;

    public void SetEventDialogue(EventDialogueSO eventDialogueSO)
    {
        _eventDialogueSO = eventDialogueSO;
        
        acceptBtn.onClick.RemoveAllListeners();
        rejectBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();
        acceptBtn.onClick.AddListener(eventDialogueSO.eventAction.Invoke);
        acceptBtn.onClick.AddListener(ExcuteDialogue);
        rejectBtn.onClick.AddListener(ExcuteDialogue);
        nextBtn.onClick.AddListener(ExcuteDialogue);
        
        ExcuteDialogue();
        _curIndex = 0;
    }

    private void ExcuteDialogue()
    {
        int dialogueCnt = _eventDialogueSO.dialogueDatas.Count;

        if (_curIndex >= dialogueCnt)
        {
            ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("Dialogue",transform);
            return;
        }
        var data = _eventDialogueSO.dialogueDatas[_curIndex];
        dialogueTxt.text = data.dialogueText;

        switch (data.dialogueType)
        {
            case DialogueType.Text:
                nextBtn.gameObject.SetActive(_curIndex < dialogueCnt - 1);
                break;
            case DialogueType.Choice:
                acceptBtn.gameObject.SetActive(true);
                rejectBtn.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _curIndex++;
    }
    
    
    
    

    public void OnSpawnFromPool()
    {
        
    }

    public void OnReturnToPool()
    {
        
    }

    public void OnDestroyFromPool()
    {
        
    }
}
