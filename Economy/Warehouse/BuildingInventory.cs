using UnityEngine;

/// <summary>
/// Локальный "склад" (буфер) для здания-производителя.
/// Хранит произведенные ресурсы до того, как их заберет тележка.
/// </summary>
public class BuildingInventory : MonoBehaviour
{
    // !! Важно: Убедись, что у тебя в проекте есть enum "ResourceType"
    // (он должен быть в ResourceManager или ResourceProductionData)
    
    [Tooltip("Какой тип ресурса производит/хранит это здание")]
    public ResourceType resourceType;
    
    [Tooltip("Максимальная вместимость этого склада")]
    public float maxAmount = 10f;

    /// <summary>
    /// Текущее количество ресурса на складе.
    /// </summary>
    public float CurrentAmount { get; private set; } = 0f;

    /// <summary>
    /// Добавляет ресурсы на склад (вызывается из ResourceProducer).
    /// </summary>
    /// <returns>True, если удалось добавить хотя бы что-то.</returns>
    public bool AddResource(float amount)
    {
        if (CurrentAmount >= maxAmount)
        {
            return false; // Склад полон
        }

        CurrentAmount += amount;
        
        if (CurrentAmount > maxAmount)
        {
            CurrentAmount = maxAmount;
        }
        
        return true;
    }

    /// <summary>
    /// Забирает ВСЕ ресурсы со склада (вызывается тележкой CartAgent).
    /// </summary>
    /// <returns>Количество ресурса, которое удалось забрать.</returns>
    public float TakeAllResources()
    {
        float amountTaken = CurrentAmount;
        CurrentAmount = 0f;
        return amountTaken;
    }

    /// <summary>
    /// Проверяет, есть ли на складе хотя бы 1 "единица" ресурса.
    /// (Используется тележкой, чтобы решить, стоит ли ехать).
    /// </summary>
    public bool HasAtLeastOneUnit()
    {
        return CurrentAmount >= 1f;
    }
}