using UnityEngine;

/// <summary>
/// BuildOrchestrator — единая «дверь» для временных состояний строительства:
/// показывает/прячет сетку, сбрасывает призраков/группы, ставит/снимает паузу производства
/// во время переноса и синхронизирует смену режимов.
/// </summary>
public class BuildOrchestrator : MonoBehaviour
{
    public static BuildOrchestrator Instance { get; private set; }

    [Header("Ссылки")]
    [SerializeField] private BuildingManager _buildingManager;
    [SerializeField] private GroupOperationHandler _groupOps;
    [SerializeField] private MassBuildHandler _massBuild;
    [SerializeField] private NotificationManager _notifications;
    
    // --- ⬇️ НОВОЕ (Шаг А, Фикс #2) ⬇️ ---
    [SerializeField] private RoadBuildHandler _roadBuildHandler; // (Нужен для CancelAll)
    // --- ⬆️ КОНЕЦ ⬆️ ---


    // Защита от рекурсии: CancelGroupOperation() → SetMode(None) → OnModeChanged(None)
    private bool _isCancelling;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Авто-поиск, если забыли проставить в инспекторе
        if (_buildingManager == null) _buildingManager = FindFirstObjectByType<BuildingManager>();
        if (_groupOps == null) _groupOps = FindFirstObjectByType<GroupOperationHandler>();
        if (_massBuild == null) _massBuild = FindFirstObjectByType<MassBuildHandler>();
        if (_notifications == null) _notifications = FindFirstObjectByType<NotificationManager>();
        
        // --- ⬇️ НОВОЕ (Шаг А, Фикс #2) ⬇️ ---
        if (_roadBuildHandler == null) _roadBuildHandler = FindFirstObjectByType<RoadBuildHandler>();
        // --- ⬆️ КОНЕЦ ⬆️ ---
    }

    /// <summary>Зови из PlayerInputController.SetMode(...) на каждый переход.</summary>
    public void OnModeChanged(InputMode newMode)
    {
        CancelAll();

        // --- ⬇️ ИЗМЕНЕНИЕ (Шаг А, Фикс #2) ⬇️ ---
        bool showGrid =
            newMode == InputMode.Building ||
            newMode == InputMode.Moving ||
            newMode == InputMode.Deleting ||
            newMode == InputMode.Upgrading ||
            newMode == InputMode.Copying ||
            newMode == InputMode.GroupMoving ||
            newMode == InputMode.GroupCopying ||
            newMode == InputMode.RoadBuilding; // <-- МЫ ДОБАВИЛИ ЭТУ СТРОКУ
        // --- ⬆️ КОНЕЦ ⬆️ ---

        _buildingManager?.ShowGrid(showGrid);
    }

    /// <summary>Универсальная аварийная очистка: призраки, группы, «кисть».</summary>
    public void CancelAll()
    {
        if (_isCancelling) return;
        _isCancelling = true;

        /*
        // (Твои старые отмены)
        _buildingManager?.CancelAllModes();
        _groupOps?.CancelGroupOperation();
        _massBuild?.ClearMassBuildPreview();
        */
        
        // --- ⬇️ НОВОЕ (Шаг А, Фикс #2) ⬇️ ---
        // Гарантированно чистим "призраки" дорог при смене режима
        _roadBuildHandler?.ClearRoadPreview();
        // --- ⬆️ КОНЕЦ ⬆️ ---


        _isCancelling = false;
    }

    /// <summary>Пауза или возврат производства на конкретном GO (используем в групповых переносах).</summary>
    public void PauseProduction(GameObject go, bool pause)
    {
        if (go == null) return;
        var prod = go.GetComponent<ResourceProducer>();
        if (prod != null) prod.enabled = !pause;
    }
}