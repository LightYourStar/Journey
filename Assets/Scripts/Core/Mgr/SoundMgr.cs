using System.Collections.Generic;

using UnityEngine;

public class EventMgr
{
    public delegate void EventListener(params object[] args);

    private static EventMgr _instance;
    public static EventMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventMgr();
            }
            return _instance;
        }
    }

    public EventMgr()
    {

    }

    private readonly object syncRoot = new object();

    private List<int> eventIdList = new List<int>();

    private Dictionary<int, List<EventListener>> eventDic = new Dictionary<int, List<EventListener>>();

    public static void AddEvent(int eventId, EventListener listener)
    {
        if (listener == null)
            return; 

        lock (Instance.syncRoot)
        {
            if (!Instance.eventDic.TryGetValue(eventId, out var list))
            {
                list = new List<EventListener>();
                Instance.eventDic[eventId] = list;
                Instance.eventIdList.Add(eventId);
            }

            if (!list.Contains(listener))
            {
                list.Add(listener);
            }
        }
    }

    public static void RemoveEvent(int eventId, EventListener listener)
    {
        if (listener == null)
            return;

        lock (Instance.syncRoot)
        {
            if (Instance.eventDic.TryGetValue(eventId, out var list))
            {
                list.Remove(listener);
                if (list.Count == 0)
                {
                    Instance.eventDic.Remove(eventId);
                    Instance.eventIdList.Remove(eventId);
                }
            }
        }
    }

    public static void DispatchEvent(int eventId, params object[] args)
    {
        List<EventListener> listenersSnapshot = null;
        lock (Instance.syncRoot)
        {
            if (Instance.eventDic.TryGetValue(eventId, out var list))
            {
                listenersSnapshot = new List<EventListener>(list);
            }
        }

        if (listenersSnapshot == null)
            return;

        foreach (var listener in listenersSnapshot)
        {
            //try
            {
                listener(args);
            }
            //catch (System.Exception ex)
            {
               // Debug.LogError($"EventMgr.DispatchEvent: listener for eventId={eventId} threw exception: {ex}");
            }
        }
    }

    public static void ClearAll()
    {
        lock (Instance.syncRoot)
        {
            Instance.eventDic.Clear();
            Instance.eventIdList.Clear();
        }
    }
}