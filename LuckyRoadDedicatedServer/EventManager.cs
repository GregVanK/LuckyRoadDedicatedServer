using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EventManager{
    public static EventManager instance;
    BinaryFormatter bf = new BinaryFormatter();
    //kinda don't know what im doing with this...
    public byte[] serializeEvent(Event e)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, e);
            return ms.ToArray();
        }
    }
    public Event deSerializeEvent(byte[] b)
    {
        using (MemoryStream ms = new MemoryStream())
        {

            ms.Write(b, 0, b.Length);
            ms.Seek(0, SeekOrigin.Begin);
            DiceEvent eventData = (DiceEvent)bf.Deserialize(ms);
            return eventData;
        }
    }
}
