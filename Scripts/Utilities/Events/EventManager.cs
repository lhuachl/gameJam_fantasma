using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de eventos pub/sub centralizado
/// Desacopla completamente los sistemas del juego
/// </summary>
public static class EventManager
{
    private static Dictionary<System.Type, List<System.Delegate>> subscribers = new Dictionary<System.Type, List<System.Delegate>>();

    /// <summary>
    /// Suscribirse a un evento específico
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        if (handler == null)
            return;

        System.Type eventType = typeof(T);
        
        if (!subscribers.ContainsKey(eventType))
            subscribers[eventType] = new List<System.Delegate>();

        subscribers[eventType].Add(handler);
        
        #if UNITY_EDITOR
        Debug.Log($"[EventManager] Subscrito a {eventType.Name} | Total: {subscribers[eventType].Count}");
        #endif
    }

    /// <summary>
    /// Desuscribirse de un evento
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        if (handler == null)
            return;

        System.Type eventType = typeof(T);
        
        if (!subscribers.ContainsKey(eventType))
            return;

        subscribers[eventType].Remove(handler);
        
        #if UNITY_EDITOR
        Debug.Log($"[EventManager] Desuscrito de {eventType.Name}");
        #endif
    }

    /// <summary>
    /// Lanzar/Broadcast un evento a todos los suscriptores
    /// </summary>
    public static void Broadcast<T>(T gameEvent) where T : GameEvent
    {
        System.Type eventType = typeof(T);
        
        if (!subscribers.ContainsKey(eventType))
        {
            #if UNITY_EDITOR
            Debug.LogWarning($"[EventManager] Evento {eventType.Name} broadcast sin suscriptores");
            #endif
            return;
        }

        List<System.Delegate> handlers = subscribers[eventType];
        
        #if UNITY_EDITOR
        Debug.Log($"[EventManager] Broadcast {eventType.Name} a {handlers.Count} handler(s)");
        #endif

        for (int i = handlers.Count - 1; i >= 0; i--)
        {
            try
            {
                var handler = handlers[i] as Action<T>;
                if (handler != null)
                    handler.Invoke(gameEvent);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EventManager] Error en handler de {eventType.Name}: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Limpiar todos los suscriptores (útil al cambiar de escena)
    /// </summary>
    public static void ClearAllSubscribers()
    {
        subscribers.Clear();
        Debug.Log("[EventManager] Todos los suscriptores limpiados");
    }

    /// <summary>
    /// Debug: Mostrar todos los eventos registrados
    /// </summary>
    public static void DebugPrintSubscribers()
    {
        Debug.Log("=== EVENTOS REGISTRADOS ===");
        foreach (var kvp in subscribers)
        {
            Debug.Log($"{kvp.Key.Name}: {kvp.Value.Count} suscriptores");
        }
    }
}
