using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AudioType
{
    BGM,SFX
}
namespace _Project.Resources.Loader
{
    [CreateAssetMenu(fileName = "AudioLoader", menuName = "AudioLoader")]
    public class AudioLoader:ScriptableObject
    {
        [Serializable]
        public class AudioData
        {
            public string audioName;
            public AudioClip clip;
            public AudioType audioType;
        }
        
        public List<AudioData> AudioDatas = new List<AudioData>();
    }
}