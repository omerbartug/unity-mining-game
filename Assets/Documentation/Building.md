# Building System

Oyuncunun satın aldığı/yerleştirdiği binaları yönetir ve otomatik üretim sisteminin temelini oluşturur.

## System Structure

```text
                         Building System
                               │
              ┌────────────────┴────────────────┐
              │                                 │
        BuildingData                         Building
          (Data)                           (Base Class)
              │                                 │
              │                    ┌────────────┴────────────┐
              │                    │                         │
              │                AutoMiner               AutoProcessor
              │                                              │
              │                                      ProcessorInputArea
              │
              └──────────────→ BuildingManager
```

### Scripts

- `BuildingData.cs` → Building'in verilerini ve ayarlarını tutar.
- `Building.cs` → Tüm building'lerin ortak temel sınıfıdır.
- `AutoMiner.cs` → Otomatik maden üretir ve depolar.
- `AutoProcessor.cs` → Input itemlarını sırayla işler ve output depolar.
- `ProcessorInputArea.cs` → Oyuncunun itemları AutoProcessor'a input olarak  vermesini sağlar.
- `BuildingManager.cs` → Building seçme, yerleştirme ve mevcut building'lere panel etkilesimini yönetir.


## Script Responsibilities

### BuildingData.cs

Bir building'in sahip olduğu verileri ve ayarları tutar.

**Responsibilities:**
- Building fiyatını tutar. → `price`
- Gerçek building prefabını tutar. → `buildingPrefab`
- Ghost prefabını tutar. → `ghostPrefab`
- Placement ayarlarını tutar. → `size`, `placementBlockerLayer`, `fineLayer`
- Production ayarlarını tutar. → `productionTime`, `storageCapacity`

---

### Building.cs

Tüm building'ler için ortak temel sınıfı oluşturur.

**Responsibilities:**
- BuildingData referansını tutar. → `buildingData`
- Production timer'ını tutar. → `timer`
- Production progress'ini sağlar. → `Progress`
- Alt sınıfların item toplama davranışını tanımlar. → `CollectItems()`

---

### AutoMiner.cs

Bağlı olduğu MiningArea'dan otomatik olarak item üretir.

**Responsibilities:**
- Bağlı olduğu MiningArea'yı bulur. → `Awake()`
- Üretim timer'ını yönetir. → `Update()`
- Storage'a üretilen itemı ekler. → `Update()`
- Biriken itemları Inventory'ye aktarır. → `CollectItems()`

---

### AutoProcessor.cs

Input itemlarını sırayla işleyerek output üretir.

**Responsibilities:**
- Input queue'sunu yönetir. → `AddInput()`
- Sıradaki itemı işler. → `Update()`
- Üretilen itemları storage'da tutar. → `Update()`
- Outputları Inventory'ye aktarır. → `CollectItems()`
- Queue değişikliğini bildirir. → `QueueChanged`
- İşlenen item değişikliğini bildirir. → `CurrentItemChanged`
- Output değişikliğini bildirir. → `OutputChanged`

---

### ProcessorInputArea.cs

Oyuncunun seçtiği processable itemları AutoProcessor'a aktarmasını sağlar.

**Responsibilities:**
- Oyuncunun seçtiği itemı alır. → `Interact()`
- Itemın processable olup olmadığını kontrol eder. → `Interact()`
- Input ekleme süresini yönetir. → `Interact()`
- Uygun itemı AutoProcessor'a gönderir. → `processor.AddInput()` (AutoProcessor prefabinin alt nesnesi olan InteractableAreanin icindedir bu metod getinparent ile hangi nesneye bagli oldugunu bulur)
- Etkileşim bırakıldığında timer'ı sıfırlar. → `ResetInteract()`

---

### BuildingManager.cs

Building seçimini ve placement sistemini yönetir.

**Responsibilities:**
- Seçili building'i belirler. → `UpdatePlacementMode()`, `SelectBuilding()`
- Ghost building oluşturur. → `SelectBuilding()`
- Ghost'u grid üzerinde hareket ettirir. → `MoveGhost()`
- Placement alanının uygunluğunu kontrol eder. → `CheckPlacement()`
- Ghost'un rengini günceller. → `UpdateGhostColor()`
- Gerçek building'i oluşturur. → `HandleLeftClick()`
- Placement modunu iptal eder. → `CancelPlacementMode()`
- Mevcut building'i seçip UI'ını açar. → `HandleLeftClick()`
 

## Dependencies

### BuildingData.cs

**Depends on:**
- `InventoryObject` → BuildingData'nın inventory itemı olarak kullanılabilmesini sağlar.
- Unity `GameObject` → Building ve ghost prefablarını tutmak için.
- Unity `LayerMask` → Placement alanlarını tanımlamak için.

**Used by:**
- `Building`
- `BuildingManager`
- `AutoMiner`
- `AutoProcessor`

---

### Building.cs

**Depends on:**
- `BuildingData` → Building'in verilerine erişmek için.
- `Inventory` → `CollectItems()` aracılığıyla itemları inventory'ye aktarmak için.

**Used by:**
- `AutoMiner`
- `AutoProcessor`
- `BuildingManager`

---

### AutoMiner.cs

**Depends on:**
- `Building` → Ortak building davranışını almak için.
- `BuildingData` → Production ve storage ayarlarını almak için.
- `MiningArea` → Üretilecek `RewardItem`ı bulmak için.
- `Inventory` → Üretilen itemları oyuncuya vermek için.

**Used by:**
- `BuildingManager`
- Building interaction systems

---

### AutoProcessor.cs

**Depends on:**
- `Building` → Ortak building davranışını almak için.
- `BuildingData` → Production ve storage ayarlarını almak için.
- `ItemData` → Input ve output itemlarını yönetmek için.
- `Inventory` → Input almak ve output vermek için.
- `ProcessorInputArea` → Oyuncudan input almak için.

**Used by:**
- `ProcessorInputArea`
- `BuildingManager`
- Processor UI

---

### ProcessorInputArea.cs

**Depends on:**
- `AutoProcessor` → Input itemlarını processor'a göndermek için.
- `Inventory` → Oyuncunun seçtiği itemı almak için.
- `ItemData` → Itemın processable olup olmadığını kontrol etmek için.
- `ProgressBar` → Input işleminin ilerlemesini göstermek için.

**Used by:**
- `AutoProcessor`
- Player interaction system

---

### BuildingManager.cs

**Depends on:**
- `Grid` → Building'i grid üzerine yerleştirmek için.
- `Inventory` → Seçili building'i almak ve yerleştirilen building'i inventory'den çıkarmak için.
- `BuildingData` → Placement ve prefab bilgilerine erişmek için.
- `BuildingUIManager` → Building UI'ını açıp kapatmak için.
- `Building` → Sahnedeki mevcut building'i bulmak için.

**Used by:**
- `PlayerInputManager`
- Building UI / player interaction systems



## GELISTIRME HEDEFI

### Sadelestirme
- Yukarida ayni fonksiyonun birden fazla gorev yaptigini veya tek scriptin bir suru is yaptigini goruyorsun.

- Bunlari duzeltmeliyiz olabildigince tek fonksiyon tek gorev / tek script tek sorumluluk ilerlemeliyiz.

- Buyuk fonksiyonlari cesitli yardimci fonksiyonlara bolerek bunlari duzelt.

- Worker isine sonra baslayacagiz.


### Optimizasyon

- Kodunda update methodu altinda saniyede 60 defa gereksiz yere calisan kodlar var. Bunlari tespit etmeliyiz.

- Tespit ettikten sonra olabildigince *event / refresh metodlari* kullanarak update metodunu optimize etmeliyiz.

- Suanki kanaatime gore update metodunda gerektigi zaman ProgressBar in anlik calismasi yeterli, digerleri eventler ile zamani gelince refresh edilseler kafi.

- Suanki bilgilerimle bu kadar oluyor 2. sinifa gecince muhtemelen farki optimiazyonlar yapacagim.