using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// BusEvent class helps subscribe/unsubscribe/publish the events for the bus design pattern.
/// </summary>
public class BusEvent
{
    private Dictionary<Type, List<EventHandler>> _subscriberByType;

    public BusEvent()
    {
        _subscriberByType = new Dictionary<Type, List<EventHandler>>();
    }
    public void Subscribe<T>(EventHandler eventHandler) where T : EventArgs
    {
        var type = typeof(T);
        if(!_subscriberByType.TryGetValue(type, out var subscribers))
        {
            _subscriberByType.Add(type, new List<EventHandler>());
        }

        _subscriberByType[type].Add(eventHandler);
    }

    public void Unsubscribe<T>(EventHandler eventHandler) where T : EventArgs
    {
        var type = typeof(T);
        if (_subscriberByType.TryGetValue(type, out var subscribers))
        {
            _subscriberByType[type].Remove(eventHandler);
        }
    }

    public void Publish(object sender, EventArgs e)
    {
        if(_subscriberByType.TryGetValue(e.GetType(), out var subscribers))
        {
            foreach (var subscriber in subscribers)
            {
                subscriber(sender, e);
            }
        }
    }
}
