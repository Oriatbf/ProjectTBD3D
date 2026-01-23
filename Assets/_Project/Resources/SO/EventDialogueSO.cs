using System;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueType
{
    Text,Choice
}

[CreateAssetMenu(fileName = "EventDialogueSO",menuName = "EventDialogueSO",order = 1)]
public class EventDialogueSO : ScriptableObject
{
    [Serializable]
    public struct DialogueData
    {
        public string dialogueText;
        public DialogueType dialogueType;
        
    }
    
    public List<DialogueData> dialogueDatas = new List<DialogueData>();
    public Action eventAction;
    
    public void SetEventAction(Action _eventAction)=>this.eventAction = _eventAction;
}
