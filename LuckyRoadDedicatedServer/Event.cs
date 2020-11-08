using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Event {
    //TODO:BoardMoveEvent, UpdateMoneyEvent, Ect?
    public enum EventType { Dice, Movement, }
    public EventType type;
}
public class DiceEvent : Event
{
    public int value;
}
