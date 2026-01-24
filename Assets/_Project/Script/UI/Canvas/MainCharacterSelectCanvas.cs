using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCharacterSelectCanvas : BaseCanvas
{
    [SerializeField] private Button selectButton;

    [SerializeField] private Transform content;
    [SerializeField]private List<MainCharacterIcon> characterIcons = new List<MainCharacterIcon>();

    private void Start()
    {
        selectButton.gameObject.SetActive(false);
        selectButton.onClick.AddListener(SelectCharacter);

        foreach (Transform child in content)
        {
            characterIcons.Add(child.GetComponent<MainCharacterIcon>());
        }

        foreach (var mainCharacterIcon in characterIcons)
        {
            mainCharacterIcon.AddCharacterBtnAction(CharacterBtnAction);
        }
    }

    private void CharacterBtnAction()
    {
        Debug.Log("CharacterBtnAction");
        selectButton.gameObject.SetActive(true);
    }

    private void SelectCharacter()
    {
        FadeInFadeOutManager.Inst.FadeOut("MapScene",true,.5f);
        DataManager.Inst.SetMainCharcter(  DataManager.Inst.Data.mainCharacterID);
        
    }
}
