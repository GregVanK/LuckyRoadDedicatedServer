using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Event
{
    //TODO:BoardMoveEvent, UpdateMoneyEvent, Ect?
    public enum EventType { Dice, Movement, }
    public EventType type;

    Event convertEvent()
    {
        switch (type)
        {
            case EventType.Dice:
                return (DiceEvent)this;
            default:
                Console.WriteLine("Failed to convert event type");
                return null;
        }

    }
}
[Serializable]
public class DiceEvent : Event
{
    public int value;

}
[Serializable]
public class MoveEvent : Event
{
    //TODO: Create Move Event
}
