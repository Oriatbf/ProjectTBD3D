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
            ChangeSFXVolume(1);
            ChangeBGMVolume(1);
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
        
        private void ChangeBGMVolume(float value)
        {
            float _volume = value == 0 ? -80f : Mathf.Log10(value) * 20;
            audioMixer.SetFloat("BGM", _volume);
        }
    
        private void ChangeSFXVolume(float value)
        {
            float _volume = value == 0 ? -80f : Mathf.Log10(value) * 20;
            audioMixer.SetFloat("SFX", _volume);
        }
    }
    
}