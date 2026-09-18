# Transport Sistemi — Implementation Plan

Transporter işçinin bir seçilmiş item'ı alıp, çizilen rota üzerinde sürekli gidip gelerek diğer işçilerin output'undan o item'ı almasını ve input'una vermesini sağlayan sistem.

## Mimari Karar

Mevcut `WorkerMovement` (tek hedefe git-dur) ve `WorkerInteraction` (timer ile etkileşim) transport davranışıyla uyuşmuyor. Bu yüzden **composition** yaklaşımıyla yeni scriptler oluşturup Worker prefab'ına ekliyoruz. Mevcut kodlara minimal dokunuş.

- `TransportMovement` → ping-pong rota hareketi (WorkerMovement'a alternatif)
- `TransportLogic` → anlık item alışverişi (WorkerInteraction'a alternatif)

---

## Adım Adım Execution Plan

### Adım 1 — TransportMovement.cs [NEW]

**Dosya:** `Assets/Scripts/Worker/TransportMovement.cs`
**Bağımlılık:** Yok

Transporter'ın çizilen rota üzerinde sürekli ping-pong yürümesini sağlar.

**Field'lar:**
- `[SerializeField] Grid grid`
- `Worker worker`
- `List<Vector3Int> routeCells` — rota hücreleri
- `int routeIndex` — şu anki hücre index'i
- `int direction = 1` — +1 ileri, -1 geri
- `Vector3 currentTarget` — hedef world position
- `bool isPatrolling`

**Method'lar:**
- `Awake()` → `worker = GetComponent<Worker>()`
- `SetRoute(List<Vector3Int> cells)` → rotayı alır, işçiyi ilk noktaya koyar, patrolü başlatır
- `StopPatrol()` → `isPatrolling = false`, rotayı temizler
- `Update()` → `MoveTowards` ile hedefe yürür, ulaşınca `AdvanceToNextWaypoint()`
- `AdvanceToNextWaypoint()` → index ilerlet, sona/başa gelince direction çevir

**Property:** `bool IsPatrolling => isPatrolling`

**Hareket:** `A(0) → B(1) → C(2) → D(3) → C(2) → B(1) → A(0) → ...`

---

### Adım 2 — TransportLogic.cs [NEW]

**Dosya:** `Assets/Scripts/Worker/TransportLogic.cs`
**Bağımlılık:** Yok

Transporter yürürken diğer worker'ların collider alanına girdiğinde anlık item alışverişi yapar. Timer yok, durmaz.

**Field'lar:**
- `Worker worker`
- `WorkerInventory inventory`
- `ItemData transportItem` — taşınacak tek item

**Method'lar:**
- `Awake()` → referansları al
- `SetTransportItem(ItemData item)` → item belirle
- `ClearTransportItem()` → item temizle
- `OnTriggerEnter2D(Collider2D other)` → lojistik akış

**Property:** `ItemData TransportItem => transportItem`

**OnTriggerEnter2D akışı:**
1. Guard: transportItem null? State != Transporting? → return
2. `other.GetComponentInParent<Worker>()` → otherWorker bul
3. Guard: null veya kendisi mi? → return
4. **BOŞALT:** `inventory.TransferToInputOf(otherInventory, transportItem)`
5. **YÜKLE:** `inventory.TransferFromOutputOf(otherInventory, transportItem)`

---

### Adım 3 — WorkerInventory.cs [MODIFY]

**Dosya:** `Assets/Scripts/Inventory/WorkerInventory.cs`
**Değişiklik:** `TransferToInputOf` altına 1 yeni method ekle

```csharp
public int TransferFromOutputOf(WorkerInventory source, InventoryObject item)
{
    if (source == null || item == null) return 0;
    if (!source.OutputItems.ContainsKey(item)) return 0;

    int available = source.OutputItems[item];
    int added = AddToOutput(item, available);
    if (added > 0)
        source.RemoveFromOutput(item, added);

    return added;
}
```

Mevcut `TransferToInputOf` karşıya **veriyor**, bu method karşıdan **alıyor**.

---

### Adım 4 — WorkerMovement.cs [MODIFY]

**Dosya:** `Assets/Scripts/Worker/WorkerMovement.cs`
**Değişiklik:** `StopMoving()` altına 1 yeni method ekle

```csharp
public void ReleaseClaim()
{
    if (hasClaimedCell)
    {
        OccupiedCells.Remove(currentCell);
        hasClaimedCell = false;
    }
}
```

Transport moduna geçerken eski hücre claim'ini serbest bırakmak için.

---

### Adım 5 — Worker.cs [MODIFY]

**Dosya:** `Assets/Scripts/Worker/Worker.cs`
**Bağımlılık:** Adım 1, 2, 4

**Yeni field'lar:**
- `TransportMovement transportMovement`
- `TransportLogic transportLogic`

**Awake'e ekle:**
- `transportMovement = GetComponent<TransportMovement>()`
- `transportLogic = GetComponent<TransportLogic>()`

**Yeni method:**
```csharp
public void StartTransporting(ItemData item, List<Vector3Int> route)
{
    CurrentState = WorkerState.Transporting;
    CurrentWorkType = WorkerWorkType.Transporting;
    if (workerMovement != null) workerMovement.ReleaseClaim();
    if (transportLogic != null) transportLogic.SetTransportItem(item);
    if (transportMovement != null) transportMovement.SetRoute(route);
    NotifyStatusChanged();
}
```

**StopWorking'e ekle (mevcut kodun altına):**
```csharp
if (transportMovement != null) transportMovement.StopPatrol();
if (transportLogic != null) transportLogic.ClearTransportItem();
```

---

### Adım 6 — RouteDrawer.cs [NEW]

**Dosya:** `Assets/Scripts/Worker/RouteDrawer.cs`
**Bağımlılık:** Yok

Sol tık basılı tutup sürükleyerek rota çizme. Sağ tık iptal.

**Field'lar:**
- `[SerializeField] Grid grid`
- `[SerializeField] NodeMaker nodeMaker`
- `[SerializeField] LineRenderer lineRenderer`
- `List<Vector3Int> routeCells`
- `bool isDrawing` — sol tık basılı mı
- `bool isActive` — çizim modu açık mı
- `Action<List<Vector3Int>> onRouteCompleted`
- `Action onRouteCancelled`

**Method'lar:**
- `StartDrawing(onCompleted, onCancelled)` → modu aktive et
- `CancelDrawing()` → modu kapat, LineRenderer temizle
- `Update()` → input loop (sol tık drag = hücre ekle, bırak = tamamla, sağ tık = iptal)
- `TryAddCellsTo(Vector3Int target)` → hızlı fare için gap-fill (manhattan yürüyüş)
- `IsWalkable(Vector3Int cell)` → NodeMaker ile kontrol
- `UpdateLineRenderer()` → routeCells'i LineRenderer'a yansıt

---

### Adım 7 — WorkerManager.cs [MODIFY]

**Dosya:** `Assets/Scripts/Managers/WorkerManager.cs`
**Bağımlılık:** Adım 6

**Yeni field'lar:**
- `[SerializeField] RouteDrawer routeDrawer`
- `bool isDrawingRoute`
- `Worker pendingTransportWorker`
- `ItemData pendingTransportItem`

**HandleLeftClick'e guard ekle:**
```csharp
if (isDrawingRoute) return;  // en başa
```

**Yeni method'lar:**
- `SetTransportMode(Worker worker, ItemData item)` → routeDrawer.StartDrawing() çağır
- `OnRouteCompleted(List<Vector3Int> route)` → worker.StartTransporting() çağır
- `OnRouteCancelled()` → state temizle

---

### Adım 8 — TransportingWorkerPanelUI.cs [NEW]

**Dosya:** `Assets/Scripts/UI/WorkerUI/TransportingWorkerPanelUI.cs`
**Bağımlılık:** Adım 2

MiningWorkerPanelUI ile aynı pattern. Transport halindeki worker'ın paneli.

**İçerik:**
- Ad, seviye, hız bilgileri
- Transport item ikonu + adı (`transportLogic.TransportItem`)
- Kargo doluluk: `outputTotal / maxOutputCapacity`
- Stop butonu → `worker.StopWorking()`
- Upgrade move speed butonu

**Event'ler:** `OnOutputChanged += RefreshCargo`, `OnStatusChanged += RefreshStatus`

---

### Adım 9 — AssignTaskPanelUI.cs [MODIFY]

**Dosya:** `Assets/Scripts/UI/WorkerUI/AssignTaskPanelUI.cs`
**Bağımlılık:** Adım 7

`OnTransportButtonClicked()` içini doldur:
```csharp
InventoryObject selected = PlayerInventory.Instance.GetSelectedItem();
if (selected == null || selected is not ItemData itemData)
{
    Debug.Log("Önce inventory'den taşınacak item'ı seç!");
    return;
}
workerManager.SetTransportMode(currentWorker, itemData);
uiManager.CloseAllPanels();
```

---

### Adım 10 — WorkerUIManager.cs [MODIFY]

**Dosya:** `Assets/Scripts/UI/WorkerUI/WorkerUIManager.cs`
**Bağımlılık:** Adım 8

**Yeni field:** `[SerializeField] TransportingWorkerPanelUI transportPanel`

**OpenWorkerUI → Transporting case:**
```csharp
case WorkerState.Transporting:
    if (transportPanel != null) transportPanel.Open(worker);
    break;
```

**CloseAllPanels'a ekle:**
```csharp
if (transportPanel != null) transportPanel.Close();
```

---

## Doğrulama

1. **Transport başlatma**: Idle worker → item seç → Transport → rota çiz → yürüsün
2. **Item alışverişi**: Madenci kömür çıkarır → Transporter taşır → Operatör'e teslim
3. **Kapasite**: Dolu transporter almayı dursun
4. **Ping-pong**: Rota sonunda geri dönsün
5. **Stop**: Panel'den durdur → Idle'a dönsün
6. **Filtre**: Sadece seçili item'ı alsın
