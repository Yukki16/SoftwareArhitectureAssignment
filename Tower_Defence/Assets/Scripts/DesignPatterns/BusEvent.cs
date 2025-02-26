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
    private Dictionary<Type, List<Delegate>> _subscriberByType;

    public BusEvent()
    {
        _subscriberByType = new Dictionary<Type, List<Delegate>>();
    }

    // Subscribe to an event
    public void Subscribe<T>(Action<T> eventHandler) where T : EventArgs
    {
        var type = typeof(T);
        if (!_subscriberByType.TryGetValue(type, out var subscribers))
        {
            _subscriberByType[type] = new List<Delegate>();
        }

        _subscriberByType[type].Add(eventHandler);
    }

    // Unsubscribe from an event
    public void Unsubscribe<T>(Action<T> eventHandler) where T : EventArgs
    {
        var type = typeof(T);
        if (_subscriberByType.TryGetValue(type, out var subscribers))
        {
            subscribers.Remove(eventHandler);
        }
    }

    // Publish an event
    public void Publish<T>(object sender, T e) where T : EventArgs
    {
        var type = typeof(T);
        if (_subscriberByType.TryGetValue(type, out var subscribers))
        {
            foreach (var subscriber in subscribers)
            {
                (subscriber as Action<T>)?.Invoke(e);
            }
        }
        Debug.Log($"Published an event of type: {e.GetType()}");
    }
}
