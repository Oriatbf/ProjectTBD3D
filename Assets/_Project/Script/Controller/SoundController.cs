using _Project.Script.Caster;
using UnityEngine;

namespace _Project.Script.Controller
{
    public class AudioController : BaseController
    {
        
        private AudioCaster _audioCaster;
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay" ,"MapScene"},
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
        }

        public void PlayAudio(string audioName)
        {
           _audioCaster.PlayAudio(audioName);
        }
    }
}