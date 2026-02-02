using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter
{
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventCenter();
            }
            return instance;
        }
    }

    private Dictionary<EventType, UnityAction<Object>> eventDic = new Dictionary<EventType, UnityAction<Object>>();

    public void AddEventListener(EventType type, UnityAction<Object> action)
    {
        if (!eventDic.ContainsKey(type))
        {
            eventDic.Add(type, action);
        }
        else
        {
            eventDic[type] += action;
        }
    }

    public void RemoveEventListenter(EventType type, UnityAction<Object> action)
    {
        if (eventDic.ContainsKey(type))
        {
            eventDic[type] -= action;
        }
    }

    public void EventTriger(EventType type, Object obj)
    {
        if (eventDic.ContainsKey(type))
        {
            eventDic[type]?.Invoke(obj);
        }
    }

    public void Clear()
    {
        eventDic.Clear();
    }


}
