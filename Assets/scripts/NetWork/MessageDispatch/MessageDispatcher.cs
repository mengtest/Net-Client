using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.NetWork;
using Assets.scripts.Message;
using Assets.scripts.NetWork.NetClient;
using C2BNet;
using System;

public class MessageDispatcher : MonoBehaviour
{
    public static List<NetMessage> msgList = new List<NetMessage>();

    // Start is called before the first frame update
    void Start()
    {
        //NetBattleClient.Instance.connect("127.0.0.1",8001);
    }

    // Update is called once per frame
    void Update()
    {
        //每帧处理消息
        for (int i = 0; i < NetConfig.MessageDispatchSpeed; i++)
        {
            if (msgList.Count > 0)
            {
                Dispatch(msgList[0]);
                lock (msgList)
                {
                    msgList.RemoveAt(0);
                }
            }
            else
            {
                break;
            }
        }
    }
    private bool Dispatch(NetMessage msg)
    {
        try
        {
            foreach (string msgtype in msg.msgtype)
            {
                MessageCenter.dispatch(msgtype, msg.data);
            }
        }
        catch (Exception ex)
			{
            
                Debug.Log( ex.Message);
            
        }
        return true;
    }
    public static bool AddTask(NetMessage msg)
    {
        lock (msgList)
        {
            msgList.Add(msg);
        }
        return true;
    }



    public class NetMessage
    {
        public IList<string> msgtype;
        public object data;
        public NetMessage(IList<string> type, object dat)
        {
            this.msgtype = type;
            this.data = dat;
        }
    }
}
