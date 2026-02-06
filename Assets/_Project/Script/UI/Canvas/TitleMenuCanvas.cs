using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Script.UI.Canvas
{
    public class TitleMenuCanvas : BaseCanvas
    {
        [SerializeField] private Button newGameBtn,loadGameBtn,tutorialBtn,exitGameBtn;
        private Action onNewGame;

        private void Start()
        {
            SetBtnHandle();
            if (DataManager.Inst.IsNewData())
            {
                loadGameBtn.enabled = false;
                loadGameBtn.transform.parent.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
            }

        }

        private void SetBtnHandle()
        {
            newGameBtn.onClick.AddListener(NewGamehandle);
            loadGameBtn.onClick.AddListener(LoadGamehandle);
            exitGameBtn.onClick.AddListener(ExitGamehandle);
            tutorialBtn.onClick.AddListener(TutorialHandle);
        }

        private void TutorialHandle()
        {
            DataManager.Inst.isTutorial = true;
            DataManager.Inst.SetCharacter(  15);
            FadeInFadeOutManager.Inst.FadeOut("GamePlay",true);
        }

        private void NewGamehandle()
        {
            onNewGame?.Invoke();
        }

        private void LoadGamehandle()
        {
            //캐릭터 선택
            FadeInFadeOutManager.Inst.FadeOut("MapScene",true);
        }

        private void ExitGamehandle()
        {
            Debug.Log("GameCloseHandle");
             #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void SetNewGameHandle(Action action)
        {
            onNewGame = action;
        }
        
        
    }
}