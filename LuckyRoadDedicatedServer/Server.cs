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
            initializeServerData();
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
                        Event e = eventManager.deSerializeEvent(incommingMessage.Data);
                        handleEvent(e,incommingMessage.SenderConnection.RemoteUniqueIdentifier)
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (incommingMessage.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                currentPlayers++;
                                addPlayer(server.Connections[currentPlayers - 1]);
                                Console.WriteLine(clients[currentPlayers].userName + " has connected");
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
        static void initializeServerData()
        {
            for (int i = 1; i <= maxPlayers; i++)
            {
                clients.Add(i ,new Client(i));
            }
        }
        static void addPlayer(NetConnection cInfo)
        {
            clients[cInfo.RemoteUniqueIdentifier].connectionInfo = cInfo;
        }

        static void handleEvent(Event e, long clientID)
        {
            switch (e.type)
            {
                case Event.EventType.Dice:
                    DiceEvent dEvent = (DiceEvent)e;
                    Console.WriteLine(clients[clientID].userName + " rolled a " + dEvent.value)
                    break;
                default:
                    Console.WriteLine("Unhandled Event Type:" + e.ToString());
                    break;

            }
        }
    }
}
