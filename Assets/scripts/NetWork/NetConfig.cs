﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.scripts.NetWork
{
    public static class NetConfig
    {
        public const int FrameTime = 50;

        public const int MessageDispatchSpeed = 15;
        public static string UdpIp = "";
        public static int UdpPort = 0;

        public static string BattleServerUDPIP = "";
        public static int BattleServerUDPPort = 0;

        public static string GameServerTCPIP = "";
        public static int GameServerTCPPort = 0;

        public const string TcpIp = "127.0.0.1";
        public const int TcpPort = 8000;
    }
}
