using System;
using System.Collections.Generic;
using _Project.Script.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Script.UI.Canvas
{
    public class DebugCanvas : BaseCanvas
    {
        [SerializeField] private Image unitSkillCheck, 
            skillIconCheck, unitCheck, untiIconCheck;
        
        private List<Image> allImage = new List<Image>();

        protected override void Awake()
        {
            base.Awake();
            allImage.Add(unitSkillCheck);
            allImage.Add(skillIconCheck);
            allImage.Add(unitCheck);
            allImage.Add(untiIconCheck);

            UnSelectAllImage();
        }


        private void Update()
        {
            if(ApplicationManager.Inst.GetModule<CharacterSkillController>().GetSkillIcon() != null)
                unitSkillCheck.color = Color.green;
            else unitSkillCheck.color = Color.red;
            
            if(ApplicationManager.Inst.GetModule<PlayerSpawnController>().GetUnitIcon() != null)
                unitCheck.color = Color.green;
            else unitCheck.color = Color.red;
        }

        private void UnSelectAllImage()
        {
            foreach (var image in allImage)
            {
                image.color = Color.red;
            }
        }
    }
}