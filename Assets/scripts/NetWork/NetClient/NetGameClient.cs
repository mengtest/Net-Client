using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

using static MessageDispatcher;
using C2GNet;
using Assets.scripts.NetWork.Service;
using Assets.scripts.Utils;
using Assets.scripts.Message;
/// <summary>
/// GameLogicLoginService
/// 
/// @Author 贾超博
/// 
/// @Date 2022/4/30
/// </summary>
namespace Assets.scripts.NetWork.NetClient
{
    public class NetGameClient
    {
		
		static NetGameClient _instance;
		public static NetGameClient Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new NetGameClient();
				}
				return _instance;
			}
		}

		TimerTask timerTask1 = null;

		Socket TcpSocket;
		byte[] TCPreadbuf = new byte[1024 * 1024];
		byte[] TCPlenBytes = new byte[sizeof(UInt32)];

		int buflen = 0;

		public void Init() {
			Connect(NetConfig.TcpIp,NetConfig.TcpPort);
			
		}

		public void StartHeartBeat() {
			timerTask1 = new TimerTask(1000, HeartBeat);
			timerTask1.execute();
		}

		private void HeartBeat() {
			
			UserService.Instance.SendHeartBeat();
		}


		public void Connect(string ip, int port)
		{
			TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				TcpSocket.Connect(ip, port);
				Debug.Log("连接服务器成功");

				StartHeartBeat();

				Start();
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
			}
		}
		void Start()
		{
			TcpSocket.BeginReceive(TCPreadbuf, 0, TCPreadbuf.Length, SocketFlags.None, StartReceiveCallback, TcpSocket);
		}
		void StartReceiveCallback(IAsyncResult ar)
		{
			try
			{
				int length = TcpSocket.EndReceive(ar);
				if (length > 0)
				{
					//buflen += length;
					//Array.Copy(TCPreadbuf, TCPlenBytes, sizeof(Int32));
					//int msgLength = BitConverter.ToInt32(TCPlenBytes, 0);
					//if (buflen >= sizeof(Int32) + msgLength) {
					//	C2GNetMessage msgs = C2GNetMessage.Parser.ParseFrom(TCPreadbuf, sizeof(Int32), msgLength);
					//	MessageDispatcher.AddTask(new NetMessage(msgs.MessageType, msgs.Response));
					//	Array.Copy(TCPreadbuf, sizeof(Int32) + msgLength, TCPreadbuf, 0, length);
					//	buflen-= sizeof(Int32) + msgLength;
					//}
					//TcpSocket.BeginReceive(TCPreadbuf, buflen, TCPreadbuf.Length- buflen, SocketFlags.None, StartReceiveCallback, TcpSocket);

					//C2GNetMessage msg = C2GNetMessage.ParseFrom(TCPreadbuf);

					//               MessageDispatcher.AddTask(new NetMessage(msg.MessageTypeList, msg.Response));

					//               TcpSocket.BeginReceive(TCPreadbuf, 0, TCPreadbuf.Length, SocketFlags.None, StartReceiveCallback, TcpSocket);

					Debug.Log("kashdkjahdsjasd");
				}
                else
                {
					Close();
				}
			}
			catch (Exception ex)
			{
				if (Connected == false)
				{
					Debug.Log("服务端断开了连接请检查网络是否连接或重启客户端，原因：" + ex.Message);
				}
				else
				{
					Debug.Log("无法接收消息：" + ex.Message);
				}
			}
		}
		public int SendMessage(C2GNetMessage msg)
		{
			try
			{
                byte[] buffer = msg.ToByteArray();

				return TcpSocket.Send(buffer);
            }
			catch (Exception ex)
			{
				if (Connected == false)
				{
					Debug.Log("无法发送消息：请检查网络连接或重启客户端，原因：" + ex.Message);
					
				}
				else
				{
					Debug.Log("无法发送消息：" + ex.Message);
				}
			}
			return -1;
		}

		public void Close()
		{
			try
			{
				MessageCenter.RemoveMsgListener(this);
				if (timerTask1 != null) { timerTask1.Stop(); }
				
				TcpSocket.Close();
			}
			catch (Exception ex)
			{
				Debug.Log("无法关闭连接：" + ex.Message);
			}
		}
		public void Reconnect() { 

		}
		public void OnLoseConnect()
		{

		}
		public bool Connected { get { return TcpSocket != null && TcpSocket.Connected == true; } }

	}
}
