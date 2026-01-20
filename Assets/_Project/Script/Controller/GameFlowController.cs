using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Script.Controller
{
    public class GameFlowController : BaseController
    {
        private RoomType _curRoomType = RoomType.Enemy;

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
                    SetEnemy();
                    break;
                case RoomType.StrongEnemy:
                    SetEnemy();
                    break;
                case RoomType.Boss:
                    SetEnemy();
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

        private void SetEnemy()
        {
            ApplicationManager.Inst.GetModule<AudioController>().PlayAudio("BattleBGM");
            ApplicationManager.Inst.GetModule<TileController>().InstanceTile();
            ApplicationManager.Inst.GetModule<CharacterSkillController>().SetCanvas();
            ApplicationManager.Inst.GetModule<PlayerSpawnController>().SetCanvas();
            FactoryManager.Inst.EnemySpawn();
        }

        private void SetShop()
        {
            var shop = UnityEngine.Resources.Load<GameObject>("Shop");
            Object.Instantiate(shop,Vector3.zero + new Vector3(0,3,0),Quaternion.identity);
        }

        private void SetEvent()
        {
            var demon =  UnityEngine.Resources.Load<GameObject>("Demon");
            var a = Object.Instantiate(demon,Vector3.zero + new Vector3(0,.7f,0),Quaternion.identity);
            //a.GetComponent<Demon>().SetEvent();
        }
    }
}