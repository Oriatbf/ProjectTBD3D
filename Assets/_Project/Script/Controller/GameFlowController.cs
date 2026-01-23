using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Script.Controller
{
    public class GameFlowController : BaseController
    {
        private RoomType _curRoomType = RoomType.Shop;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _curRoomType = DataManager.Inst.GetCurRoomType();
            SetFLow();
        }

        private void SetFLow()
        {
            switch (_curRoomType)
            {
                case RoomType.Village:
                    break;
                case RoomType.Enemy:
                    SetEnemy(EnemyArrangeType.enemy);
                    break;
                case RoomType.StrongEnemy:
                    SetEnemy(EnemyArrangeType.strongEnemy);
                    break;
                case RoomType.Boss:
                    SetEnemy(EnemyArrangeType.Boss);
                    break;
                case RoomType.Event:
                    SetEvent();
                    break;
                case RoomType.Shop:
                    SetShop();
                    break;
                case RoomType.Rebellion:
                    break;
                case RoomType.None:
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
        }

        private void SetShop()
        {
            var shop = UnityEngine.Resources.Load<GameObject>("Shop");
            Object.Instantiate(shop,Vector3.zero + new Vector3(0,3,0),Quaternion.identity);
            ApplicationManager.Inst.GetModule<TurnController>().MapStage();
        }

        private void SetEvent()
        {
            var demon =  UnityEngine.Resources.Load<GameObject>("Demon");
            var a = Object.Instantiate(demon,Vector3.zero + new Vector3(0,.7f,0),Quaternion.identity);
            ApplicationManager.Inst.GetModule<TurnController>().MapStage();
        }
    }
}