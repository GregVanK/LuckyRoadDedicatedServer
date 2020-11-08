using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuckyRoadDedicatedServer
{
    class Client
    {
        public int id;
        public string userName;
        public string ipAddress;
        private NetConnection ConnectionInfo;
        public NetConnection connectionInfo
        {
            get
            {
                return ConnectionInfo;
            }
            set
            {
                this.ConnectionInfo = value;
                peer = ConnectionInfo.Peer;
                if(ConnectionInfo.RemoteHailMessage != null)
                    userName = ConnectionInfo.RemoteHailMessage.ReadString();
                else if(ConnectionInfo.LocalHailMessage != null)
                        userName = ConnectionInfo.LocalHailMessage.ReadString();
            }
        }
        public NetPeer peer;
        public Client(int i)
        {
            id = i;
        }
        
    }
}
