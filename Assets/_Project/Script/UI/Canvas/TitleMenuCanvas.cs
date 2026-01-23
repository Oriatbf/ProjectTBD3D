using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Script.UI.Canvas
{
    public class TitleMenuCanvas : BaseCanvas
    {
        [SerializeField] private Button newGameBtn,loadGameBtn,exitGameBtn;

        private void Start()
        {
            SetBtnHandle();
            if (DataManager.Inst.IsNewData())
            {
                loadGameBtn.enabled = false;
            }

        }

        private void SetBtnHandle()
        {
            newGameBtn.onClick.AddListener(NewGamehandle);
            loadGameBtn.onClick.AddListener(LoadGamehandle);
            exitGameBtn.onClick.AddListener(ExitGamehandle);
        }

        private void NewGamehandle()
        {
            //캐릭터 선택
            FadeInFadeOutManager.Inst.FadeOut("MapScene",true);
        }

        private void LoadGamehandle()
        {
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
        
        
    }
}