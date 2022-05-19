using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.scripts.Models;
using Assets.scripts.NetWork.NetClient;
using Assets.scripts.Utils;
using C2BNet;
/// <summary>
/// GameLogicLoginService
/// 
/// @Author 贾超博
/// 
/// @Date 2022/4/30
/// </summary>

namespace Assets.scripts.NetWork.Service
{
    public class GameLogicService
    {
        private static GameLogicService _instance;
        public static GameLogicService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameLogicService();
                   
                }
                return _instance;
            }
        }
        
        public void init()
        {
            
        }

        public void SendFrameHandle(FrameHandlesFromClient frameHandles)
        {
            //LogUtil.log("SendFrameHandle",frameHandle);
            var userId = User.Instance.user.Id;

            frameHandles.ToBuilder().SetUserId(userId);

            var Net = new C2BNetMessage.Builder()
            {
                Request = new C2BNetMessageRequest.Builder()
                {
                    UserId = userId,
                    FrameHandles = frameHandles
                }.Build()
            }.Build();

            NetBattleClient.Instance.SendMessage(Net);
        }


        /**
         * 发送进度转发
         */
        public void SendPercentForward(int percent)
        {
            LogUtil.log("SendPercentForward");
            var userId = User.Instance.user.Id;
            var Net = new C2BNetMessage.Builder()
            {
                Request = new C2BNetMessageRequest.Builder()
                {
                    UserId = userId,
                    PercentForward = new PercentForward.Builder()
                    {
                        UserId = userId,
                        Percent = percent
                    }.Build()
                }.Build()
            }.Build();

            NetBattleClient.Instance.SendMessage(Net);

        }


        /**
        * 发送游戏结束
        */
        public void SendGameOver()
        {
            //LogUtil.log("SendGameOver");
            var userId = User.Instance.user.Id;
            var Net = new C2BNetMessage.Builder()
            {
                Request = new C2BNetMessageRequest.Builder()
                {
                    UserId = userId,
                    GameOverReq = new GameOverRequest.Builder().Build()
                }.Build()
            }.Build();

            NetBattleClient.Instance.SendMessage(Net);


        }

        /**
            * 发送补帧请求
            */
        public void SendRepairFrame(int startFrame, int endFrame)
        {
            // LogUtil.log("SendRepairFrame");
            var userId = User.Instance.user.Id;

            var Net = new C2BNetMessage.Builder()
            {
                Request = new C2BNetMessageRequest.Builder()
                {
                    UserId = userId,
                    RepairFrameReq = new RepairFrameRequest.Builder()
                    {
                        StartFrame = startFrame,
                        EndFrame = endFrame
                    }.Build()
                }.Build()
            }.Build();

            NetBattleClient.Instance.SendMessage(Net);


        }

        /**
         * 记录用户请求
         */
        public void SendRecordUser()
        {
            var userId = User.Instance.user.Id;
            //console.log('SendRecordUser')
            var Net = new C2BNetMessage.Builder()
            {
                Request = new C2BNetMessageRequest.Builder()
                {
                    UserId = userId
                }.Build()
            }.Build();

            NetBattleClient.Instance.SendMessage(Net);

        }

    }
}
