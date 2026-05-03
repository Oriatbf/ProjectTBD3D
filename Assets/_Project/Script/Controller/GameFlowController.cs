using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Script.Controller
{
    public class GameFlowController : BaseController
    {
        private NodeType curNodeType = NodeType.Event;

        public override void OnInitialize()
        {
            base.OnInitialize();
            curNodeType = DataManager.Inst.GetMapData().prevNodeCoord.type;
            SetFLow();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(Input.GetKeyDown(KeyCode.F))
                Debug.Log(curNodeType);
        }

        private void SetFLow()
        {
            if (DataManager.Inst.isTutorial)
            {
                curNodeType = NodeType.Tutorial;
                DataManager.Inst.isTutorial = false;
            }
            switch (curNodeType)
            {
                case NodeType.Village:
                    break;
                case NodeType.Enemy:
                    SetEnemy(EnemyArrangeType.enemy);
                    break;
                case NodeType.StrongEnemy:
                    SetEnemy(EnemyArrangeType.strongEnemy);
                    break;
                case NodeType.Boss:
                    SetEnemy(EnemyArrangeType.Boss);
                    break;
                case NodeType.MidBoss:
                    SetEnemy(EnemyArrangeType.MidBoss);
                    break;
                case NodeType.TestEnemy:
                    SetEnemy(EnemyArrangeType.TestEnemy);
                    break;
                case NodeType.Event:
                    SetEvent();
                    break;
                case NodeType.Shop:
                    SetShop();
                    break;
                case NodeType.Rebellion:
                    break;
                case NodeType.None:
                    break;
                case NodeType.Tutorial:
                    SetEnemy(EnemyArrangeType.Tutorial); 
                    ApplicationManager.Inst.GetModule<TutorialController>().StartTutorial("Battle");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetEnemy(EnemyArrangeType enemyArrangeType)
        {
            ApplicationManager.Inst.GetModule<AudioController>().PlayAudio("BattleBGM");
            ApplicationManager.Inst.GetModule<TileController>().InstanceTile();
            ApplicationManager.Inst.GetModule<CharacterSkillController>().SetCanvas();
            ApplicationManager.Inst.GetModule<PlayerSpawnController>().SetCanvas();
            FactoryManager.Inst.EnemySpawn(enemyArrangeType);
            ApplicationManager.Inst.GetModule<SkillProgressController>().RegisterTutorial();
        }

        private void SetShop()
        {
            ApplicationManager.Inst.GetModule<AudioController>().PlayAudio("ShopBGM");
            var shop = UnityEngine.Resources.Load<GameObject>("Shop");
            Object.Instantiate(shop,Vector3.zero + new Vector3(0,3,0),Quaternion.identity);
            ApplicationManager.Inst.GetModule<TurnController>().MapStage();
        }

        private void SetEvent()
        {
            ApplicationManager.Inst.GetModule<AudioController>().PlayAudio("ShopBGM");
            var demon =  UnityEngine.Resources.Load<GameObject>("Demon");
            var a = Object.Instantiate(demon,Vector3.zero + new Vector3(0,.7f,0),Quaternion.identity);
            ApplicationManager.Inst.GetModule<TurnController>().MapStage();
        }

        public NodeType GetCurNodeType() => curNodeType;
    }
}