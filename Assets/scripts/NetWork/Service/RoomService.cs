﻿using Assets.scripts.GameLogic;
using Assets.scripts.Managers;
using Assets.scripts.Message;
using Assets.scripts.Models;
using Assets.scripts.NetWork.NetClient;
using Assets.scripts.UI.Common;

using Assets.scripts.Utils;
using C2GNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.scripts.Utils.enums.BattleModeEnum;

namespace Assets.scripts.NetWork.Service
{
    public class RoomService
    {
        private static RoomService _instance;
        public static RoomService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoomService();

                }
                return _instance;
            }
        }


        public void init()
        {
            MessageCenter.AddMsgListener(MessageType.OnMyRoom, this.OnMyRoom, this);
            MessageCenter.AddMsgListener(MessageType.OnInviteResponse, this.OnInviteResponse, this);
            MessageCenter.AddMsgListener(MessageType.OnInviteRequest, this.OnInviteRequest, this);
            MessageCenter.AddMsgListener(MessageType.OnKickOut, this.OnKickOut, this);
            MessageCenter.AddMsgListener(MessageType.OnRoomStartGame, this.OnRoomStartGame, this);
            MessageCenter.AddMsgListener(MessageType.OnNickNameSearch, this.OnNickNameSearch, this);
            MessageCenter.AddMsgListener(MessageType.OnAddRoomRequest, this.OnAddRoomRequest, this);
            MessageCenter.AddMsgListener(MessageType.OnAddRoomResponse, this.OnAddRoomResponse, this);
            MessageCenter.AddMsgListener(MessageType.OnOutRoom, this.OnOutRoom, this);
            MessageCenter.AddMsgListener(MessageType.OnAddLiveResponse, this.OnAddLiveResponse, this);
            MessageCenter.AddMsgListener(MessageType.OnValidateOpenRoom, this.OnValidateOpenRoom, this);
        }
        /**
        * 请求我的房间
        */
        public void SendMyRoom()
        {
            //LogUtil.log("SendMyRoom");
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    MyRoomReq = new MyRoomRequest.Builder().Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);
        }

        /**
     * 我的房间响应
     */
        private void OnMyRoom(object param)
        {
            var response = param as MyRoomResponse;
            //LogUtil.log("OnMyRoom:{0}", response.room);
            MessageCenter.dispatch(MessageType.OnMyRoom_UI, response.Room);
        }

        /**
     * 发送邀请请求
     */
        public void SendInviteRequest(int toUserId, string toNickName, int teamId)
        {
            //LogUtil.log("SendInviteRequest");
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    InviteReq = new InviteRequest.Builder()
                    {
                        FromUserId = User.Instance.user.Id,
                        FromNickName = User.Instance.user.Nickname,
                        ToUserId = toUserId,
                        ToNickName = toNickName,
                        TeamId = teamId

                    }.Build()

                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);

        }

        /**
         * 发送邀请响应
         */
        public void SendInviteResponse(bool accept, InviteRequest inviteRequest)
        {
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    InviteRes = new InviteResponse.Builder()
                    {


                        Resultmsg = accept ? Result.Success : Result.Failed,
                        Errormsg = "",
                        InviteRequest = inviteRequest
                    }.Build()
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);

        }
        /**
    * 收到邀请请求
    */
        private async void OnInviteRequest(object param)
        {
            var request = param as InviteRequest;
            //LogUtil.log("OnInviteRequest", request);
            var confirmObj = await TipsManager.Instance.Show(request.FromNickName + "邀请你加入房间？", "邀请请求", MessageBoxType.Confirm, "接受", "拒绝");
            var this_ = this;
            MessageCenter.AddMsgListener(MessageType.UIMessageBox_OnClickYes, (p) => { SendInviteResponse(true, request); }, confirmObj);
            MessageCenter.AddMsgListener(MessageType.UIMessageBox_OnClickNo, (p) =>
            {
                this_.SendInviteResponse(false, request);
            }, confirmObj);
        }

        /**
         * 收到邀请响应
         */
        private void OnInviteResponse(object param)
        {
            var response = param as InviteResponse;
            //LogUtil.log("OnInviteResponse:{0}{1}", response.result,response.errormsg);
            /**************************
            TipsManager.Instance.showTips(response.errormsg);
            */
            if (response.Resultmsg == Result.Success)
            {
                //被邀请者是当前用户
                if (response.InviteRequest.ToUserId == User.Instance.user.Id)
                {
                    /**************************
                director.loadScene('Room');
                    */
                }
                else
                {
                    MessageCenter.dispatch(MessageType.OnMyRoom_RefieshUI, 0);
                }
            }
        }

        /**
     * 踢出响应
     */
        private void OnKickOut(object param)
        {
            var response = param as KickOutResponse;
            //LogUtil.log("OnKickOut:{0}{1}", response.result,response.errormsg);
            // EventManager.Instance.dispatch(EventType.OnKickOut_UI,response); 
            MessageCenter.dispatch(MessageType.OnMyRoom_RefieshUI, 0);
            TipsManager.Instance.showTips(response.Errormsg);
        }

        /**
         * 开始游戏请求
         */
        public void SendRoomStartGame()
        {
            //LogUtil.log("SendRoomStartGame");
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    RoomStartGameReq = new RoomStartGameRequest.Builder().Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);


        }

        /**
         * 开始游戏响应
         */
        public void OnRoomStartGame(object param)
        {
            var response = param as RoomStartGameResponse;
            //LogUtil.log("OnRoomStartGame{0}{1}：", response.result, response.errormsg);
            MessageCenter.dispatch(MessageType.OnRoomStartGame_UI, response);
            TipsManager.Instance.showTips(response.Errormsg);
        }

        /**
         * 昵称搜索请求
         */
        public void SendNickNameSearch(string nickName)
        {
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    NickNameSearchReq = new NickNameSearchRequest.Builder()
                    {
                        NickName = nickName
                    }.Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);

        }

        /**
        * 昵称搜索响应
*/
        public void OnNickNameSearch(object param)
        {
            var response = param as NickNameSearchResponse;
            //LogUtil.log("OnNickNameSearch");
            MessageCenter.dispatch(MessageType.OnNickNameSearch_UI, response.RoomUserList);
        }

        /**
         * 发送加入房间请求
         */
        public void SendAddRoomRequest(int roomId)
        {
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    AddRoomReq = new AddRoomRequest.Builder()
                    {
                        RoomId = roomId,
                        FromUserId = User.Instance.user.Id,
                        FromNickName = User.Instance.user.Nickname

                    }.Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);

        }

        /**
         * 发送加入房间响应
         */
        public void SendAddRoomResponse(bool accept, int teamId, AddRoomRequest addRoomRequest)
        {
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    AddRoomRes = new AddRoomResponse.Builder()
                    {
                        Result = accept ? Result.Success : Result.Failed,
                        Errormsg = "",
                        TeamId = teamId,
                        AddRoomRequest = addRoomRequest
                    }.Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);
            //LogUtil.log("SendAddRoomResponse");

        }

        /**
         * 收到加入房间请求
         */
        private async void OnAddRoomRequest(object param)
        {
            var request = param as AddRoomRequest;
            //LogUtil.log("OnAddRoomRequest", request);
            var confirmObj = await TipsManager.Instance.Show(request.FromNickName + "加入房间？", "加入房间", MessageBoxType.Confirm, "接受", "拒绝");
            var this_ = this;
            MessageCenter.AddMsgListener(MessageType.UIMessageBox_OnClickYes, async (p) =>
            {
                var teamConfirmObj = await TipsManager.Instance.Show("请选择" + request.FromNickName + "加入队伍！", "选择队伍", MessageBoxType.Confirm, "友队", "敌队");
                MessageCenter.AddMsgListener(MessageType.UIMessageBox_OnClickYes, (p) =>
                {
                    this_.SendAddRoomResponse(true, 0, request);
                }, teamConfirmObj);
                MessageCenter.AddMsgListener(MessageType.UIMessageBox_OnClickNo, (p) =>
                {
                    this_.SendAddRoomResponse(true, 1, request);
                }, teamConfirmObj);
            }, confirmObj);
            MessageCenter.AddMsgListener(MessageType.UIMessageBox_OnClickNo, /*async */(p) =>
            {
                this_.SendAddRoomResponse(false, 0, request);
            }, confirmObj);
        }


        /**
         * 收到加入房间响应
         */
        private void OnAddRoomResponse(object param)
        {
            var response = param as AddRoomResponse;
            //LogUtil.log("OnAddRoomResponse:{0}{1}", response.Result, response.Errormsg);
            TipsManager.Instance.showTips(response.Errormsg);
            if (response.Result == Result.Success)
            {
                //加入者是当前用户
                if (response.AddRoomRequest.FromUserId == User.Instance.user.Id)
                {
                    /*******************************
            //director.loadScene('Room');
                    ******************************************/
                }
                else
                {
                    MessageCenter.dispatch(MessageType.OnMyRoom_RefieshUI, 0);
                }
            }
        }

        /**
         * 退出房间请求
         */
        public void SendOutRoom()
        {
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    OutRoomReq = new OutRoomRequest.Builder().Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);
            //LogUtil.log("SendOutRoom");

        }

        /**
         * 退出房间响应
         */
        public void OnOutRoom(object param)
        {
            var response = param as OutRoomResponse;
            //LogUtil.log("OnOutRoom{0}{1}：", response.result, response.errormsg);
            TipsManager.Instance.showTips(response.Errormsg);
        }

        /**
         * 请求游戏结束
         */
        public void SendGameOver2()
        {
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    GameOver2Req = new GameOver2Request.Builder()
                    {
                        IpPortStr = User.Instance.room.IpPortStr
                    }.Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);
            //LogUtil.log("SendGameOver2");
        }

        /**
         * 上传比分请求
         */
        public void SendUploadBiFen(string biFen)
        {
            //LogUtil.log("SendUploadBiFen");
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    UploadBiFenReq = new UploadBiFenRequest.Builder()
                    {
                        BiFen = biFen
                    }.Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);
        }

        /**
         * 进入直播请求
         */
        public void SendAddLive(int targetUserId)
        {
            //LogUtil.log("SendAddLive");
            GameLogicGlobal.targetLiveUserId = targetUserId;
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    AddLiveReq = new AddLiveRequest.Builder()
                    {
                        UserId = targetUserId
                    }.Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);
        }

        /**
         * 收到进入直播响应
         */
        private void OnAddLiveResponse(object param)
        {
            var response = param as AddLiveResponse;
            //LogUtil.log("OnAddLiveResponse:{0}{1}", response.result, response.errormsg);
            TipsManager.Instance.showTips(response.Errormsg);
            if (response.Result == Result.Success)
            {  //进入直播房间
                LocalStorageUtil.RemoveItem(LocalStorageUtil.allFrameHandlesKey);
                GameLogicGlobal.battleMode = BattleMode.Live;

                User.Instance.room = response.Room;
                RandomUtil.seed = response.Room.RandomSeed;   //设置战斗随机数种子

                //director.loadScene('EnterGameLoad');
            }
        }

        /**
         * 请求效验是否可以开房间
         */
        public void SendValidateOpenRoom()
        {
            //LogUtil.log("SendValidateOpenRoom");
            var Net = new C2GNetMessage.Builder()
            {
                Request = new NetMessageRequest.Builder()
                {
                    ValidateOpenRoomReq = new ValidateOpenRoomRequest.Builder().Build(),
                }.Build()
            }.Build();

            NetGameClient.Instance.SendMessage(Net);

        }

        /**
         * 效验是否可以开房间响应
         */
        private void OnValidateOpenRoom(object param)
        {
            var response = param as ValidateOpenRoomResponse;
            //LogUtil.log("OnValidateOpenRoom:{0}", response);
            if (response.Result == Result.Success)
            {
                //director.loadScene('Room');
            }
            else
            {
                TipsManager.Instance.showTips(response.Errormsg);
            }
        }

    }
}
