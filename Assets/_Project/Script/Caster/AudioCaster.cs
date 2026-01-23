using System;
using System.Collections.Generic;
using _Project.Resources.Loader;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Script.Caster
{
    public class AudioCaster : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource,sfxSource;
        [SerializeField] private AudioMixer audioMixer;
        AudioLoader _audioLoader;
        List<AudioLoader.AudioData> _audioDatas = new List<AudioLoader.AudioData>();
        private void Awake()
        {
            _audioLoader = UnityEngine.Resources.Load<AudioLoader>("Loader/AudioLoader");
            if(_audioLoader==null)Debug.LogError("AudioLoader is null");
            _audioDatas = _audioLoader.AudioDatas;
            if(_audioDatas==null)Debug.LogError("AudioDatas is null");
        }

        public void PlayAudio(string audioName)
        {
            var targetData = _audioDatas.Find(a => a.audioName == audioName);
            var targetClip = targetData.clip;
            switch (targetData.audioType)
            {
                case AudioType.BGM:
                    bgmSource.clip = targetClip;
                    bgmSource.Play();
                    break;
                case AudioType.SFX:
                    sfxSource.PlayOneShot(targetClip);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void ChangeMasterVolume(float value)
        {
            DataManager.Inst.SaveMasterVolume(value);
            float _volume = value == 0 ? -80f : Mathf.Log10(value) * 20;
            audioMixer.SetFloat("Master", _volume);
        }
        
        public void ChangeBGMVolume(float value)
        {
            DataManager.Inst.SaveBGMVolume(value);
            float _volume = value == 0 ? -80f : Mathf.Log10(value) * 20;
            audioMixer.SetFloat("BGM", _volume);
        }
        
    
        public void ChangeSFXVolume(float value)
        {
            DataManager.Inst.SaveSFXVolume(value);
            float _volume = value == 0 ? -80f : Mathf.Log10(value) * 20;
            audioMixer.SetFloat("SFX", _volume);
        }
    }
    
}