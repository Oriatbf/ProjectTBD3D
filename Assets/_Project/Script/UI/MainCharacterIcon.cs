using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainCharacterIcon : MonoBehaviour
{
    [SerializeField] private Button characterBtn;
    [SerializeField] private int characterID;

    private void Start()
    {
        characterBtn.onClick.AddListener(CharacterSelectHandle);
    }

    public void AddCharacterBtnAction(Action action)
    {
        characterBtn.onClick.AddListener(action.Invoke);
    }

    private void CharacterSelectHandle()
    {
        DataManager.Inst.Data.mainCharacterID = characterID;
    }
}
