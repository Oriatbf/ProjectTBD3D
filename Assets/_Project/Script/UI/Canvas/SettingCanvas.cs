using System;
using _Project.Script.Controller;
using Core.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

public class SettingCanvas : BaseCanvas
{
    [SerializeField] private Transform menuContent,settingContent;
    [SerializeField] private Button backBtn;
    [Foldout("MenuContentSetting")]
    [SerializeField] private Button backToGameBtn,settingBtn,gameCloseBtn;
    [EndFoldout]
    
    [Foldout("SettingContent")]
    [SerializeField] private Scrollbar masterScrollbar,bgmScrollbar,sfxScrollbar;
    
    private GameObject lastSelected;
    private AudioController _audioController;

    protected override void Awake()
    {
        base.Awake();
        _audioController = ApplicationManager.Inst.GetModule<AudioController>();
        SetAction();
        SetAudioHandler();
        lastSelected = backToGameBtn.gameObject;
    }

    protected override void Show(bool isDotween, bool isRaycast, float tweenDuration)
    {
        base.Show(isDotween, isRaycast, tweenDuration);
        EventSystem.current.SetSelectedGameObject(backToGameBtn.gameObject);
        menuContent.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(false);
        settingContent.gameObject.SetActive(false);
    }

    private void SetAction()
    {
        backToGameBtn.onClick.AddListener(BackToGameHandle);
        backToGameBtn.onClick.AddListener(DataManager.Inst.JsonSave);
        settingBtn.onClick.AddListener(SettingHandle);
        gameCloseBtn.onClick.AddListener(GameCloseHandle);
        backBtn.onClick.AddListener(BackBtnHandle);
    }
    void Update()
    {
        if (!isShow) return;
        var es = EventSystem.current;
        if (es == null) return;

        if (es.currentSelectedGameObject == null)
        {
            if (lastSelected != null)
                es.SetSelectedGameObject(lastSelected);
        }
        else
        {
            lastSelected = es.currentSelectedGameObject;
        }
    }
    
    private void BackToGameHandle()
    {
        if (!isShow) return;
        Debug.Log("BackToGameHandle");
        ChangeState(false, true);
    }

    private void SettingHandle()
    {
        if (!isShow) return;
        Debug.Log("SettingHandle");
        menuContent.gameObject.SetActive(false);
        settingContent.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
        SetSlider();
        
    }

    private void SetSlider()
    {
        if (!isShow) return;
        var soundData = DataManager.Inst.GetSoundData();
        masterScrollbar.value = soundData.masterVolume;
        bgmScrollbar.value = soundData.bgmVolume;
        sfxScrollbar.value = soundData.sfxVolume;
    }

    private void GameCloseHandle()
    {
        Debug.Log("GameCloseHandle");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void BackBtnHandle()
    {
        backBtn.gameObject.SetActive(false);
        settingContent.gameObject.SetActive(false);
        menuContent.gameObject.SetActive(true);
    }

    private void SetAudioHandler()
    {
        var audioCaster = _audioController.GetAudioCaster();
        masterScrollbar.onValueChanged.AddListener(audioCaster.ChangeMasterVolume);
        bgmScrollbar.onValueChanged.AddListener(audioCaster.ChangeBGMVolume);
        sfxScrollbar.onValueChanged.AddListener(audioCaster.ChangeSFXVolume);
    }
}
