using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCharacterSelectCanvas : BaseCanvas
{
    [SerializeField] private Button selectButton,tutorialSelectBtn,tutorialUnSelectBtn;

    [SerializeField] private Transform content, tutorialPopUp;
    [SerializeField]private List<MainCharacterIcon> characterIcons = new List<MainCharacterIcon>();

    private void Start()
    {
        tutorialPopUp.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(false);
        
        tutorialSelectBtn.onClick.AddListener(SelectTutorial);
        selectButton.onClick.AddListener(SelectCharacter);
        tutorialUnSelectBtn.onClick.AddListener(Map);

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

    private void SelectTutorial()
    {
        DataManager.Inst.SetMainCharcter(  1);
        DataManager.Inst.GetMapData().curNodeCoord = new NodeCoord(0,0,NodeType.Tutorial);
        FadeInFadeOutManager.Inst.FadeOut("GamePlay",true,.5f);
    }

    private void Map()
    {
        FadeInFadeOutManager.Inst.FadeOut("MapScene",true,.5f);
        DataManager.Inst.SetMainCharcter(  1);
    }

    private void SelectCharacter()
    {
        if (DataManager.Inst.IsFirstGame())
        {
            tutorialPopUp.gameObject.SetActive(true);
        }
        else
        {
            Map();
        }
    }
}
