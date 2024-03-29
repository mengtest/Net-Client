﻿using Assets.scripts.GameLogic;
using Assets.scripts.Message;
using Assets.scripts.Models;
using Assets.scripts.NetWork;
using Assets.scripts.NetWork.NetClient;
using Assets.scripts.NetWork.Service;
using Assets.scripts.Utils;
using C2BNet;
using C2GNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.scripts.Utils.enums.BattleModeEnum;

namespace Assets.scripts.UI.UIPanels
{
    public class UIEnterGameLoad:BaseUIForm
    {
        private TimerTask timer=null;
        private int percent_ = 0;   //加载进度百分比
        private bool isGoToBattleScene = false; //是否已跳转战斗场景
        public void Start()
        {
           
        }
        public void Awake()
        {
            
        }

        public void Init()
        {

            this.InitTeamUser(User.Instance.room);

            //连接到战斗服务器
            var ipPortArr = User.Instance.room.IpPortStr.Split(":");
            NetConfig.UdpIp = ipPortArr[0];
            NetConfig.UdpPort = int.Parse(ipPortArr[2]);

            //console.log('战斗服务器地址：'+NetConfig.websocketBattleUrl)

            NetBattleClient.Instance.Connect(NetConfig.UdpIp, NetConfig.UdpPort);

            MessageCenter.AddMsgListener(MessageType.OnPercentForward, this.OnPercentForward, this);

            if (GameLogicGlobal.battleMode == BattleMode.Battle)
            {  //对局模式
               //上传加载进度，需要等所有用户资源都加载完成
                timer = new TimerTask(5000, () =>
                {
                    //console.log('uploadProgress percent_=' + this_.percent_)
                    GameLogicService.Instance.SendPercentForward(this.percent_);
                    if (this.percent_ < 100)
                    {
                        this.percent_ += 20;
                    }
                });
            }
            else if (GameLogicGlobal.battleMode == BattleMode.Live)
            {  //观看直播模式
               //加载资源，只需要当前用户的资源加载完成即可
               //跳转战斗场景

                //director.loadScene('Map01');

            }
        }

        private void OnPercentForward(object param)
        {
            var response = param as PercentForwardResponse;
            //console.log("OnPercentForward:{0} [{1}]", response.percentForward,response.allUserLoadSucess);
            var userId = response.PercentForward.UserId;
            var percent = response.PercentForward.Percent;

            this.UpdateTeamUserPercent(userId, percent);

            //全部用户资源加载成功
            if (response.AllUserLoadSucess && !this.isGoToBattleScene)
            {
                this.isGoToBattleScene = true;

                //director.loadScene('Map01');  //跳转战斗场景

            }
        }
        /**
        * 初始化队伍信息
        * @param roomUserList 
        * @param teamType 
        */
        private void InitTeamUser(NRoom roomUserList)
        {
            //let avatar = (teamType == TeamType.My ? 'myAvatar' : 'enemyAvatar');
            //let nickname = (teamType == TeamType.My ? 'myNickname' : 'enemyNickname');
            //for (let i = 0; i < roomUserList.length; i++)
            //{
            //    let roomUser = roomUserList[i];
            //    //console.log('roomUser.userId=' + roomUser.userId)
            //    //this[avatar + (i + 1)].spriteFrame = await LoadResUtil.load_local_sprite(DataManager.Instance.characters[roomUser.cCharacterId].UpperBodyImg + '/spriteFrame');
            //    //this[nickname + (i + 1)].string= roomUser.nickName;
            //}
        }

        /**
         * 更新队伍用户加载进度
         * @param userId 
         * @param percent 
         * @param teamType 
         */
        private void UpdateTeamUserPercent(int userId, int percent)
        {

            foreach (AllTeam allTeam in User.Instance.room.AllTeamList) {
                foreach (RoomUser roomUser in allTeam.TeamList) {
                    if (roomUser.UserId == userId)
                    {
                        //this[percentName + (i + 1)].string= percent + '%';
                    }
                }
            }
        }

        public override void Close()
        {
            MessageCenter.RemoveMsgListener(this);
            CloseUIForm();
        }
    }
}
