using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Мозг" (синглтон), который отслеживает все склады (Warehouse) на карте.
/// Тележки обращаются к нему, чтобы найти ближайший пункт назначения.
/// </summary>
public class WarehouseManager : MonoBehaviour
{
    // --- Синглтон ---
    public static WarehouseManager Instance { get; private set; }

    // --- Данные ---
    // Список всех активных складов в игре
    private List<Warehouse> _allWarehouses = new List<Warehouse>();

    void Awake()
    {
        // Базовый паттерн синглтона
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // (Можно добавить DontDestroyOnLoad(gameObject); если менеджер один на всю игру)
        }
    }

    /// <summary>
    /// Склад "встает на учет" при появлении на сцене.
    /// Вызывается из Warehouse.cs в OnEnable().
    /// </summary>
    public void Register(Warehouse wh)
    {
        if (!_allWarehouses.Contains(wh))
        {
            _allWarehouses.Add(wh);
        }
    }

    /// <summary>
    /// Склад "снимается с учета" при уничтожении.
    /// Вызывается из Warehouse.cs в OnDisable().
    /// </summary>
    public void Unregister(Warehouse wh)
    {
        if (_allWarehouses.Contains(wh))
        {
            _allWarehouses.Remove(wh);
        }
    }

    /// <summary>
    /// Находит ближайший склад к указанной точке (позиции тележки).
    /// </summary>
    /// <param name="position">Точка, от которой ищем (обычно transform.position тележки)</param>
    /// <returns>Ближайший Warehouse или null, если складов нет.</returns>
    public Warehouse GetNearestWarehouse(Vector3 position)
    {
        if (_allWarehouses.Count == 0)
        {
            return null; // Складов нет
        }

        Warehouse nearest = null;
        float minDistance = float.MaxValue;

        // Пробегаемся по всему списку и ищем самый близкий
        foreach (Warehouse wh in _allWarehouses)
        {
            float distance = Vector3.Distance(position, wh.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = wh;
            }
        }

        return nearest;
    }
}