using System;
using Lidgren.Network;
using System.Threading;
namespace LuckyRoadDedicatedServer
{
    class Server
    {
        static NetServer server;
        static void Main(string[] args)
        {
            Console.Title = "Lucky Road Dedicated Server";

            NetPeerConfiguration config = new NetPeerConfiguration("LuckyRoad");
            config.MaximumConnections = 100;
            config.Port = 32450;
            //config.EnableUPnP = true;
            //i dont fully know what this does, but if we have dropped packets, probably try disabling this (its suppose to reduce the amount of memory usage)
            //config.AutoFlushSendQueue = true;


            server = new NetServer(config);
            server.Start();
            while (true)
            {
                serverLoop();
            }
        }

        static void serverLoop()
        {
            NetIncomingMessage incommingMessage;
            while ((incommingMessage = server.ReadMessage()) != null)
            {
                switch (incommingMessage.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // handle custom messages
                        var data = incommingMessage.ReadString();
                        Console.WriteLine(data);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (incommingMessage.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                Console.WriteLine("User has connected");
                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine("User has disconnected");
                                break;
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine(incommingMessage.ReadString());
                        break;

                    default:
                        Console.WriteLine("unhandled message with type: "
                            + incommingMessage.MessageType);
                        break;
                }
                server.Recycle(incommingMessage);
            }
            Thread.Sleep(ServerSettings.TICK_RATE);
        }
    }
}
