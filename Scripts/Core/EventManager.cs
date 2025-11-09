using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// EventManager - Sistema de eventos centralizado
/// FASE 2: Comunicación desacoplada
/// 
/// Permite que sistemas se comuniquen sin conocerse entre sí
/// 
/// Uso:
///   EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);
///   EventManager.Broadcast(new BossDefeatedEvent { bossId = "boss1" });
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Suscribirse a un evento
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        if (Instance == null) return;

        Type eventType = typeof(T);
        if (Instance.eventDictionary.TryGetValue(eventType, out Delegate existingDelegate))
        {
            Instance.eventDictionary[eventType] = Delegate.Combine(existingDelegate, handler);
        }
        else
        {
            Instance.eventDictionary[eventType] = handler;
        }

        Debug.Log($"EventManager: Subscrito a {eventType.Name}");
    }

    /// <summary>
    /// Desuscribirse de un evento
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        if (Instance == null) return;

        Type eventType = typeof(T);
        if (Instance.eventDictionary.TryGetValue(eventType, out Delegate existingDelegate))
        {
            Delegate updatedDelegate = Delegate.Remove(existingDelegate, handler);
            if (updatedDelegate != null)
            {
                Instance.eventDictionary[eventType] = updatedDelegate;
            }
            else
            {
                Instance.eventDictionary.Remove(eventType);
            }

            Debug.Log($"EventManager: Desuscrito de {eventType.Name}");
        }
    }

    /// <summary>
    /// Broadcast de un evento - todos los suscriptores se notifican
    /// </summary>
    public static void Broadcast<T>(T gameEvent) where T : GameEvent
    {
        if (Instance == null) return;

        Type eventType = typeof(T);
        if (Instance.eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            (eventDelegate as Action<T>)?.Invoke(gameEvent);
            Debug.Log($"EventManager: Broadcasteado {eventType.Name}");
        }
    }
}
