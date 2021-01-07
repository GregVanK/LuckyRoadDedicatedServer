using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;

namespace LuckyRoadDedicatedServer
{
    public enum PacketTypes
    {
        LocalPlayerPacket,
        PlayerDisconnectsPacket,
        PositionPacket,
        DicePacket,
        TurnStatePacket,
        SpawnPacket
    }

    public interface IPacket
    {
        void PacketToNetOutGoingMessage(NetOutgoingMessage message);
        void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public abstract class Packet : IPacket
    {
        public abstract void PacketToNetOutGoingMessage(NetOutgoingMessage message);
        public abstract void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public class LocalPlayerPacket : Packet
    {
        public string ID { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.LocalPlayerPacket);
            message.Write(ID);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            ID = message.ReadString();
        }
    }

    public class PlayerDisconnectsPacket : Packet
    {
        public string player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.PlayerDisconnectsPacket);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();
        }
    }

    public class PositionPacket : Packet
    {
        public float X { get; set; }
        public float Z { get; set; }
        public string player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.PositionPacket);
            message.Write(X);
            message.Write(Z);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            X = message.ReadFloat();
            Z = message.ReadFloat();
            player = message.ReadString();
        }
    }
    public class DicePacket : Packet
    {
        public int Dice { get; set; }
        public string player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.DicePacket);
            message.Write(Dice);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Dice = message.ReadInt32();
            player = message.ReadString();
        }
    }

    //Pass the turn state to the server to help manidate the flow of the game
    public class TurnStatePacket : Packet
    {
        public enum GameState
        {
            TurnStart,
            DiceRoll,
            TurnEnd
        }
        public GameState state { get; set; }
        public string player { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.DicePacket);
            message.Write((byte)state);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            state = (GameState)message.ReadByte();
            player = message.ReadString();
        }
    }

    public class SpawnPacket : Packet
    {
        public float X { get; set; }
        public float Z { get; set; }
        public string player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.SpawnPacket);
            message.Write(X);
            message.Write(Z);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            X = message.ReadFloat();
            Z = message.ReadFloat();
            player = message.ReadString();
        }
    }
}
