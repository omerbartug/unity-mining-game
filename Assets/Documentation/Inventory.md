# Inventory System

> Oyuncunun sahip olduğu itemleri ve miktarlarını yönetir.
> Inventory verinin sahibidir; UI sadece bu veriyi gösterir.

## System Structure

### Scripts

- `Inventory.cs` → Inventory verisini yönetir.
- `InventorySlot.cs` → Tek bir slotun item ve miktar bilgisini tutar.
- `InventoryUI.cs` → Inventory verisini ekranda gösterir ve slot seçimini yönetir.
- `InventorySlotUI.cs` → Tek bir slotun görselini ve tıklamasını yönetir.

### Structure

```text
Inventory
│
├── InventorySlot[]
│   └── Data + Amount
│
└── InventoryUI
    └── InventorySlotUI[]
```


## Script Responsibilities

### Inventory.cs

Oyuncunun inventory verisini yönetir.

**Responsibilities:**
- 8 inventory slotunu yönetir.
- Item ekler ve çıkarır. `AddItem()` , `RemoveItem()`
- Oyuncunun belirli bir itema sahip olup olmadığını kontrol eder. `HasItem()`
- Seçili itema erişim sağlar. `GetSelectedItem()` , `GetSelectedSlot()`
- Inventory değiştiğinde UI'ın yenilenmesini sağlar. (UI referansindan Refresh() metodu ile)

---

### InventorySlot.cs
 
Tek bir inventory slotunun verisini tutar.

**Contains:**
- `Data` → Slotta bulunan item.
- `Amount` → Item miktarı.

**Responsibilities:**
- O slotun itemini ve amountun gelen emirle degistirmek, metodlarin nedeni encapsulation.

---

### InventoryUI.cs

Inventory verisini gorselini yonetir ve seçili slotu yönetir.

**Responsibilities:**
- 8 adet `InventorySlotUI` oluşturur. (Awake kisminda Instantiate ediyor)
- Inventory verisini UI'a aktarır. `Refresh()`
- Slot seçimini yönetir. `SelectSlot()`
- Slot secimini yonetmesinin sebebi eski secilen slotun indexini bilmesidir.
- Inventory değiştiğinde UI'ı yeniler.

---

### InventorySlotUI.cs

Tek bir inventory slotunun görselini yönetir.

**Responsibilities:**
- Item ikonunu gösterir.
- Item miktarını gösterir.
- Seçili slotun border'ını gösterir.
- Slot tıklamasını `InventoryUI`'a bildirir. `OnPointerClick()`   