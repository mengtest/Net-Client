using Assets.scripts.GameLogic;
using Assets.scripts.Core.GameLogic;
using Assets.scripts.GameLogic.Managers;
using Assets.scripts.Message;
using Assets.scripts.Models;
using Assets.scripts.NetWork;
using Assets.scripts.NetWork.Service;
using Assets.scripts.UI.UIPanels;
using Assets.scripts.Utils;
using Assets.scripts.Utils.enums;
using C2BNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.scripts.Utils.enums.BattleModeEnum;
using static Assets.scripts.Utils.enums.GameStatusEnum;
using static Assets.scripts.Utils.enums.HandlerFrameResultEnum;
using static Assets.scripts.Utils.enums.OptTypeEnum;

namespace Assets.scripts.Managers
{
    public class GameLogicManager:MonoBehaviour
    {
        public static GameLogicManager _instance;
        public static GameLogicManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameLogicManager();
                }
                return _instance;
            }
        }
        public  FrameHandlesFromClient.Builder frameHandles = new FrameHandlesFromClient.Builder();  //玩家帧操作对象
        public int handleFrameId=-1;  //已经处理的帧
        public int executeFrameId=-1;  //已经执行的帧
        //key：帧id  value：玩家操作集合
        public static SortedDictionary<int,IList<FrameHandle>> allFrameHandles = new SortedDictionary<int, IList<FrameHandle>>();  //所有的帧操作
        private TimerTask timer;
        private TimerTask recProTimer;
        private TimerTask handleFrameTimer;
        private TimerTask recordUserTimer;


        public int NextOperationNum=1;
        public static List<FrameHandle> PredictedInput=new List<FrameHandle>();


        private IGameCoreLogic gameLogic = new GameCoreLogic();

        public GameStatus gameStatus =GameStatus.None;  //游戏状态
     
        private int  repairWaitFrameCount=5*7;  //补帧等待帧数
        private int currentRepairFrame=0;  //当前执行补帧

        public int newFrameId = -1;  //最新帧
        // public isRecProFlag:boolean = true; //是否恢复进度中
    
        private int liveNotExecuteFrameCount = 0;  //直播未执行帧数
    
        private float lastReceiveFrameTime = 0; //最后接收帧时间
        private float lastCheckFrameTime = 0; //最后抽查时间

        public bool isAddListener = false; //是否开始监听事件

        public void Awake()
        {
            
        }
        public void Start()
        {
            
        }
        public void Update()
        {
            if (isAddListener) {
                HandlerFrameResult handlerFrameResult = OnHandlerFrame();
                RepairFrameRequest(handlerFrameResult);
            }
        }
        public void Clear() {
            handleFrameTimer.Stop();
            recordUserTimer.Stop();

        }
        public async void init() {
            UIGameLoadIn uIGameLoadIn= (UIGameLoadIn)UIManager.GetInstance() .ShowUIForms("");
            uIGameLoadIn.setMsg("游戏拼命加载中...");
            


            this.isAddListener = false;
            this.Clear();
            //Debug.Log('BattleManager start creatureMap len：' + CreatureManager.Instance.creatureMap.values().length)
            //创建角色
            await CharacterManager.Instance.CreateCharacter();  //蓝队
            
                                                                             //创建怪物
            //await SpawnMonManager.Instance.Init();

            //
            //

            //
            //
            //小地图初始化
            //UIMinimapManager.Instance.Init();

            //初始化技能提示模型
            //for (let i = 0; i < this.skillTipsPrefabs.length; i++)
            //{
            //    let node = instantiate(this.skillTipsPrefabs[i]) as Node;
            //    BattleGlobal.skillTipsMap.put(node.name, node);
            //}

            this.gameStatus = GameStatus.GameIn;
            MessageCenter.dispatch(MessageType.OnBattleGameIn,0);

            var allFrameHandlesStr = LocalStorageUtil.GetItem(LocalStorageUtil.allFrameHandlesKey);
            if (allFrameHandlesStr!=null)
            {  //恢复进度
                //console.log('恢复进度')

                //allFrameHandles = JSON.parse(allFrameHandlesStr).map || { };

                //    let this_=this;
                //    this.recProTimer=setInterval(async function(){
                //     await this_.IntervalProgressRecovery(this_);
                //   }, 0);  //2
            }





            StartMonitorFrame();
        }

        private void StartMonitorFrame()
        {
            MessageCenter.AddMsgListener(MessageType.OnFrameHandle, this.OnFrameHandle, this);
            MessageCenter.AddMsgListener(MessageType.OnRepairFrame, this.OnRepairFrame, this);
            MessageCenter.AddMsgListener(MessageType.OnLiveFrame, this.OnLiveFrame, this);
            //console.log('StartMonitorFrame')
            this.isAddListener = true;

            if (GameLogicGlobal.battleMode == BattleMode.Battle)
            {    //对局模式
                handleFrameTimer = new TimerTask(NetConfig.FrameTime, CapturePlayerOpts);
                handleFrameTimer.execute();
            }
            else if (GameLogicGlobal.battleMode == BattleMode.Live)
            {  //观看直播模式


            }

            recordUserTimer = new TimerTask(1000, () => {
                if (this.handleFrameId >= 0)
                {
                    recordUserTimer.Stop();
                    // this_.isRecProFlag = false;
                 }
                GameLogicService.Instance.SendRecordUser();
            });
        }
        /**
            * 帧操作响应
            */
        public void OnFrameHandle(object obj)
        {
            FrameHandleResponse param= obj as FrameHandleResponse;
            //计算接收两帧之间的时间间隔
            float currentFrameTime = Time.time;
            if (this.lastReceiveFrameTime != 0 && currentFrameTime - this.lastCheckFrameTime > 3000)
            {  //每3秒抽查下
                var ms = currentFrameTime - this.lastReceiveFrameTime;

                //this.uiBattle.updateFrameTime(ms);

                this.lastCheckFrameTime = currentFrameTime;
            }
            this.lastReceiveFrameTime = currentFrameTime;


            var response = param;

            var frameId = response.Frame;

            this.newFrameId = frameId;

            if (this.newFrameId - 50 > this.handleFrameId)
            {
                //this.uiGameLoadIn.setMsg('游戏进度恢复中...');
                //this.uiGameLoadIn.show();
            }
            else
            {
                //this.uiGameLoadIn.hide();
            }

            //已经处理的帧
            if (frameId <= this.handleFrameId)
            {
                return;
            }
            if (!allFrameHandles.ContainsKey(frameId))
            {
                allFrameHandles.Add(frameId, response.FrameHandlesList);//收到帧保存起来
            }
        }

        /**
         * 直播帧响应
         * @param param 
         */
        public void OnLiveFrame(object obj)
        {
            LiveFrameResponse param= obj as LiveFrameResponse;    
            // let response = param[0] as LiveFrameResponse;
            var response = param;
            var liveFrames = response.LiveFramesList;
            for (int i = 0; i < liveFrames.Count; i++)
            {
                var liveFrame = liveFrames[i];
                if (!allFrameHandles.ContainsKey(liveFrame.Frame)) {
                    allFrameHandles.Add(liveFrame.Frame, liveFrame.FrameHandlesList);
                }
                    
            }
            // this.liveNotExecuteFrameCount += liveFrames.length;
        }

        private void OnRepairFrame(object obj)  {
            RepairFrameResponse response = obj as RepairFrameResponse;
            // console.log("OnRepairFrame:{0}", JSON.stringify(response.repairFrames));
            foreach (RepairFrame repairFrame in response.RepairFramesList) {
                if (!allFrameHandles.ContainsKey(repairFrame.Frame)) {
                    allFrameHandles.Add(repairFrame.Frame, repairFrame.FrameHandlesList);
                }
            }
        }
        
    





         private HandlerFrameResult OnHandlerFrame() {
            var frameId = this.handleFrameId + 1;

            

            //获取帧操作集合
            IList<FrameHandle> frameHandles = allFrameHandles[frameId];
            // console.log(this.allFrameHandles.length+'==='+frameId)
            // console.log('OnHandlerFrame='+frameId+'，'+frameHandles)
            if (frameHandles==null)
            {  //无帧数据
                return HandlerFrameResult.NoFrameData;
            }
            if (this.executeFrameId >= frameId)
            {
                //Debug.log('不能重复执行，已经执行的帧：' + this.executeFrameId)
                return HandlerFrameResult.NotRepeatFrame;
            }
            this.executeFrameId = frameId;


            gameLogic.update(frameHandles);



            this.handleFrameId = frameId;   //更新已经同步的帧

            //缓存帧数据
            if (frameId % 15 == 0)
            {
                //LocalStorageUtil.SetItem(LocalStorageUtil.allFrameHandlesKey, JSON.stringify(allFrameHandles));
            }
            return HandlerFrameResult.Success;
         }

        /**
         * 补帧效验
         * @param handlerFrameResult 
         * @return  是否补帧了
         */
        private void RepairFrameRequest(HandlerFrameResult handlerFrameResult) {
        if(handlerFrameResult == HandlerFrameResult.NoFrameData){
            if(this.currentRepairFrame <= 0){
            //补帧请求
            var start = this.handleFrameId + 1;
                    var end = this.GetEndFrameId(start);
            if ((end- start)<10)
            {
                return ;
            }
            //console.log('补帧请求 start=' + start + '，' + end + '，handleFrameId=' + this.handleFrameId)
                GameLogicService.Instance.SendRepairFrame(start, end);
            this.currentRepairFrame = this.repairWaitFrameCount;
        }else{
            this.currentRepairFrame--;
        }
            return;
        }
        this.currentRepairFrame=0;
        return;
    }
        /**
        * 获取补帧结束帧
        * @param startFrameId 起始帧
        */
        private int GetEndFrameId(int startFrameId)
        {
            var frameIds = allFrameHandles.Keys;
           
           
            return frameIds.Max();
        }


        public void CapturePlayerOpts(){
            //无操作
            if(this.frameHandles.FrameHandlesCount==0){
               return;
            }

            this.frameHandles.SetUserId(User.Instance.user.Id);
           // LogUtil.log(this.frameHandle);
            //发送操作

            GameLogicService.Instance.SendFrameHandle(this.frameHandles.Build());

            this.frameHandles.Clear();
            
        }

        public void AddPlayerOpt(FrameHandle frameHandle)
        {
            frameHandle.ToBuilder().SetUserId(User.Instance.user.Id).SetOpretionId(NextOperationNum++);
            PredictedInput.Add(frameHandle);
            frameHandles.AddFrameHandles(frameHandle);
        }

        public void OnDestroy()
        {
            Clear();
            
          
            Destroy(this);
        }
    }
}
