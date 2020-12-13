using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace LuckyRoadDedicatedServer
{
    public class PlayerPosition
    {
        public float X { get; set; }
        public float Z { get; set; }
    }
    public class Server
    {
        private NetServer server;
        private Thread thread;
        private List<string> players;
        private Dictionary<string, PlayerPosition> playerPositions;

        public Server()
        {
            players = new List<string>();
            playerPositions = new Dictionary<string, PlayerPosition>();

            NetPeerConfiguration config = new NetPeerConfiguration("LuckyRoad");
            config.MaximumConnections = 4;
            config.Port = 32450;

            server = new NetServer(config);
            server.Start();

            thread = new Thread(Listen);
            thread.Start();
        }

        public void Listen()
        {
            Logger.Info("Listening for clients...");
            
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage message;

                while ((message = server.ReadMessage()) != null)
                {
                    Logger.Info("Message received");

                    //Get list of users
                    List<NetConnection> all = server.Connections;

                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();

                            string reason = message.ReadString();

                            if (status == NetConnectionStatus.Connected)
                            {
                                var player = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
                                // add player to dictionary
                                players.Add(player);

                                // send player local ID
                                SendLocalPlayerPacket(message.SenderConnection, player);

                                // send spawn info
                                spawnPlayers(all, message.SenderConnection, player);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            //Get packet type
                            byte type = message.ReadByte();

                            //Create packet
                            Packet packet;

                            switch(type)
                            {
                                case (byte)PacketTypes.PositionPacket:
                                    packet = new PositionPacket();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendPositionPacket(all, (PositionPacket)packet);
                                    break;
                                case (byte)PacketTypes.PlayerDisconnectsPacket:
                                    packet = new PlayerDisconnectsPacket();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendPlayerDisconnectPacket(all, (PlayerDisconnectsPacket)packet);
                                    break;
                                default:
                                    Logger.Error("Unhandled data / Packet type");
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = message.ReadString();
                            Logger.Debug(text);
                            break;
                        default:
                            Logger.Error("Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes " + message.DeliveryMethod);
                            break;
                    }

                    server.Recycle(message);
                }
            }
        }

        public void spawnPlayers(List<NetConnection> all, NetConnection local, string player)
        {
            // Spawn all clients on the local player
            all.ForEach(p =>
            {
                string _player = NetUtility.ToHexString(p.RemoteUniqueIdentifier);

                if (player != _player)
                {
                    SendSpawnPacketToLocal(local, _player, playerPositions[_player].X, playerPositions[_player].Z);
                }
            });

            // Spawn local player on all clients
            Random random = new Random();
            SendSpawnPacketToAll(all, player, random.Next(-3, 3), random.Next(-3, 3));
        }

        public void SendLocalPlayerPacket(NetConnection local, string player)
        {
            Logger.Info("Sending player their user ID: " + player);

            NetOutgoingMessage message = server.CreateMessage();
            new LocalPlayerPacket() { ID = player }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public void SendSpawnPacketToLocal(NetConnection local, string player, float X, float Z)
        {
            Logger.Info("Sending user spawn info for player: " + player);

            playerPositions[player] = new PlayerPosition() { X = X, Z = Z };

            NetOutgoingMessage message = server.CreateMessage();
            new SpawnPacket() { player = player, X = X, Z = Z }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendSpawnPacketToAll(List<NetConnection> all, string player, float X, float Z)
        {
            Logger.Info("Sending user spawn info for player: " + player);

            playerPositions[player] = new PlayerPosition() { X = X, Z = Z };

            NetOutgoingMessage message = server.CreateMessage();
            new SpawnPacket() { player = player, X = X, Z = Z }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendPositionPacket(List<NetConnection> all, PositionPacket packet)
        {
            Logger.Info("Sending position for " + packet.player);

            playerPositions[packet.player] = new PlayerPosition { X = packet.X, Z = packet.Z };
            
            NetOutgoingMessage message = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendPlayerDisconnectPacket(List<NetConnection> all, PlayerDisconnectsPacket packet)
        {
            Logger.Info("Player disconnected: " + packet.player);
            playerPositions.Remove(packet.player);
            players.Remove(packet.player);

            NetOutgoingMessage message = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }

        /*static NetServer server;
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
        }*/
    }
}
