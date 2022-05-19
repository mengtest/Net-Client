using Assets.scripts.Managers;
using Assets.scripts.Models;
using Assets.scripts.NetWork.NetClient;
using Assets.scripts.NetWork.Service;
using Assets.scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// GameLogicLoginService
/// 
/// @Author 贾超博
/// 
/// @Date 2022/4/30
/// </summary>
namespace Assets.scripts
{
    public class LoadManager
    {
        private static LoadManager _instance;
        public static LoadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoadManager();

                }
                return _instance;
            }
        }

        public bool init() {
            try {
                //DateUtil.InitExtend();
                User.Instance.isLogin = false;
                //UIManager.GetInstance().Init();
                //DataManager.Instance.Load();
                NetGameClient.Instance.Init();
                NetBattleClient.Instance.Init();


                UserService.Instance.init();
                //StatusService.Instance.Init();


                TipsService.Instance.Init();
                RoomService.Instance.init();
                //FollowManager.Instance.Init();
                ChatService.Instance.init();
                MatchService.Instance.init();
                GameLogicService.Instance.init();
                //FollowService.Instance.Init();
                //CombatPowerRankingManager.Instance.Init();

                return true;
            }
            catch (Exception ex) { return false; }
           
        }

        public bool Close() {
            try
            {
                NetGameClient.Instance.Close();
                NetBattleClient.Instance.Close();
                LocalStorageUtil.RemoveItem(LocalStorageUtil.allFrameHandlesKey);
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}
