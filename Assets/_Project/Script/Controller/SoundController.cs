using _Project.Script.Caster;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Script.Controller
{
    public class AudioController : BaseController
    {
        
        private AudioCaster _audioCaster;
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay" ,"MapScene","Title"},
            Priority = 0,
            UpdateInterval = 1,
            LateUpdateInterval = 1,
            FixedUpdateInterval = 1,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            var obj = UnityEngine.Resources.Load<GameObject>("AudioCaster");
            var audioCaster = Object.Instantiate(obj);
            _audioCaster = audioCaster.GetComponent<AudioCaster>();
            if(_audioCaster == null)Debug.Log("AudioCaster spawn not found");
            SetDataVolume();
            SetTitleBgm();
        }

        private void SetTitleBgm()
        {
            if(SceneManager.GetActiveScene().name == "Title")
                _audioCaster.PlayAudio("TitleBGM");
        }

        private void SetDataVolume()
        {
            var soundData = DataManager.Inst.GetSoundData();
            _audioCaster.ChangeMasterVolume(soundData.masterVolume);
            _audioCaster.ChangeBGMVolume(soundData.bgmVolume);
            _audioCaster.ChangeSFXVolume(soundData.sfxVolume);
        }

        public void PlayAudio(string audioName)
        {
           _audioCaster.PlayAudio(audioName);
        }
        
        public AudioCaster GetAudioCaster()
        {
            if(_audioCaster == null)Debug.LogError("AudioCaster is null");
            return _audioCaster;
        }
    }
}