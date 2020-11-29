using System;
using Lidgren.Network;
using System.Threading;
using System.Collections.Generic;

namespace LuckyRoadDedicatedServer
{
    class Server
    {
        static NetServer server;
        public static Dictionary<long, Client> clients;
        public static int maxPlayers;
        private static int currentPlayers = 0;
        private static EventManager eventManager = new EventManager();
        static void Main(string[] args)
        {
            Console.Title = "Lucky Road Dedicated Server";

            NetPeerConfiguration config = new NetPeerConfiguration("LuckyRoad");
            config.MaximumConnections = 100;
            config.Port = 32450;

            clients = new Dictionary<long, Client>();
            maxPlayers = 4;
            
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
                        
                        handleEvent(incommingMessage);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (incommingMessage.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                currentPlayers++;
                                addPlayer(server.Connections[currentPlayers - 1]);
                                Console.WriteLine(clients[server.Connections[currentPlayers - 1].RemoteUniqueIdentifier].userName + " has connected");
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
        static void addPlayer(NetConnection cInfo)
        {
            clients.Add(cInfo.RemoteUniqueIdentifier, new Client(cInfo.RemoteHailMessage.ReadString()));
        }

        static void handleEvent(NetIncomingMessage eventMessage)
        {
            Event e = eventManager.deSerializeEvent(eventMessage.Data);
            long sender = eventMessage.SenderConnection.RemoteUniqueIdentifier;

            switch (e.type)
            {
                //Username system broke...
                case Event.EventType.Dice:
                    DiceEvent dEvent = (DiceEvent)e;
                   Console.WriteLine("SOMEONE rolled a " + dEvent.value);
                    List<NetConnection> all = server.Connections;
                    all.Remove(eventMessage.SenderConnection);

                    if(all.Count > 0)
                    {
                        NetOutgoingMessage relayMsg = server.CreateMessage();
                        relayMsg.Write(eventManager.serializeEvent(dEvent));
                        server.SendMessage(relayMsg, all, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled Event Type:" + e.ToString());
                    break;

            }
        }
    }
}
