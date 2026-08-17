# Worker System

# GELISTIRME PLANLARI

```text

                         WORKER SYSTEM
                              │
                              ▼
                    ┌─────────────────────┐
                    │  1. WORKER TEMELİ   │
                    └─────────────────────┘
                              │
                              ├── Worker GameObject
                              ├── Worker.cs
                              ├── Worker Stats
                              ├── Worker Movement
                              └── Worker Interaction
                              │
                              ▼
                    ┌─────────────────────┐
                    │ 2. WORKER INVENTORY │
                    └─────────────────────┘
                              │
                              ├── Inventory (Base)
                              │      │
                              │      ├── PlayerInventory
                              │      └── WorkerInventory
                              │
                              ├── AddItem
                              ├── RemoveItem
                              ├── Capacity
                              └── Item taşıma
                              │
                              ▼
                    ┌─────────────────────┐
                    │  3. WORKER SEÇİMİ   │
                    └─────────────────────┘
                              │
                              ├── Sol tık → Worker seç
                              ├── WorkerManager
                              └── SelectedWorker
                              │
                              ▼
                    ┌─────────────────────┐
                    │    4. WORKER UI      │
                    └─────────────────────┘
                              │
                              ├── Worker'a tıkla
                              ├── Worker Panel aç
                              ├── Worker bilgileri
                              └── [Mining] [Transport] ...
                              │
                              ▼
                    ┌─────────────────────┐
                    │    5. TASK SYSTEM    │
                    └─────────────────────┘
                              │
                              ├── WorkerTask
                              │
                              ├── MiningTask
                              ├── TransportTask
                              └── SellTask
                              │
                              ▼
                    ┌─────────────────────┐
                    │  6. MINING TASK      │
                    └─────────────────────┘
                              │
                              ├── Oyuncu Mining seçer
                              ├── UI kapanır
                              ├── "Ore Tile seç"
                              ├── Oyuncu Ore Tile'a tıklar
                              ├── Worker hedefi alır
                              ├── Worker Ore'a gider
                              └── Mining başlar
                              │
                              ▼
                    ┌─────────────────────┐
                    │ 7. ORE ASSIGNMENT   │
                    └─────────────────────┘
                              │
                              ├── 1 Ore Tile = 1 Worker
                              ├── WorkerAssigned
                              └── Aynı tile'a ikinci Worker yok
                              │
                              ▼
                    ┌─────────────────────┐
                    │  8. WORKER MINING   │
                    └─────────────────────┘
                              │
                              ├── WorkerInteract
                              ├── MiningArea.Interact(...)
                              ├── Worker mining speed
                              ├── Ore üret
                              ├── WorkerInventory'ye koy
                              └── Carry: 10 / 10
                              │
                              ▼
                    ┌─────────────────────┐
                    │ 9. CAPACITY / STATE │
                    └─────────────────────┘
                              │
                              ├── Carry capacity
                              ├── Idle
                              ├── Moving
                              ├── Working
                              └── Full
                              │
                              ▼
                    ┌─────────────────────┐
                    │ 10. TRANSPORT TASK  │
                    └─────────────────────┘
                              │
                              └── SONRA TASARLAYACAĞIZ