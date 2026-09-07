# Transporter ve İki Kademeli (Input/Output Paylaşımlı) İşçi Sistemi - Uygulama Yol Haritası

Bu doküman, sistemin **baştan sona, en sağlam bağımlılık sırasıyla ve bir daha arkaya dönülmeyecek şekilde** inşa edilmesi için hazırlanmış detaylı uygulama planıdır.

---

## 1. Mimari Prensipler ve Tasarım Kararları

```text
                           [WORKER INVENTORY MİMARİSİ]
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │  TOPLAM KAPASİTE: 30 (Upgrade edildikçe 30 -> 33 -> 36 artar)              │
 ├──────────────────────────────────────┬──────────────────────────────────────┤
 │          GİRDİ (INPUT BUFFER)        │         ÇIKTI (OUTPUT BUFFER)        │
 │           Max: 3 Slot                │              Max: 3 Slot             │
 │                                      │                                      │
 │   [Slot 0]   [Slot 1]   [Slot 2]     │   [Slot 0]   [Slot 1]   [Slot 2]     │
 │    Kömür      Demir      (Boş)       │   Külçe       (Boş)      (Boş)       │
 │     10         20         0          │     30          0          0         │
 └──────────────────────────────────────┴──────────────────────────────────────┘
  * Kural: Toplam item sayısı <= Kapasite. 
  * Madenci/Taşıyıcı tek ürünle çalıştığında 30 kapasitenin 30'unu da tek slotta kullanır.
  * Operatör ise 30 kapasiteyi farklı hammaddeler arasında serbestçe paylaştırabilir.
```

### Transporter Alış-Veriş Kuralları:
1. **Tek Odak İtem (`designatedItem`)**: Her transporter sadece tek bir item türünü taşır (Örn: `Demir Cevheri`).
2. **Boşaltma Zorunluluğu (Unload to Input)**: Yanından geçtiği işçinin Input buffer'ında boş yer varsa ve bu item'ı kabul ediyorsa, elindekini verebildiği kadar boşaltmak zorundadır.
3. **Yükleme Zorunluluğu (Load from Output)**: Yanından geçtiği işçinin Output buffer'ında taşıdığı odak item varsa ve kendi kargo kapasitesinde yer varsa, alabildiği kadar almak zorundadır.

---

## 2. Adım Adım Uygulama Sırası (Phased Implementation)

### Aşama 1: Çekirdek Envanter Modeli (Foundation - WorkerInventory)
> **Amaç**: Sistemin omurgasını kurmak. Diğer tüm sistemler bu envantere veri yazıp okuyacağı için ilk yapılması gereken yerdir.

- **[MODIFY] [WorkerInventory.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Inventory/WorkerInventory.cs)**:
  - Eski `Dictionary` yapısı kaldırılır.
  - `InventorySlot[] inputSlots = new InventorySlot[3];` ve `InventorySlot[] outputSlots = new InventorySlot[3];` tanımlanır.
  - `currentInputTotal` ve `currentOutputTotal` O(1) sayaçları eklenir.
  - **Paylaşımlı Kapasite Mantığı**: Yeni item eklenirken `currentTotal < carryCapacity` kontrolü yapılır; aynı item varsa o slot büyür (30'a kadar), yoksa boş slota yerleşir.
  - **Güvenli Takas Metotları**:
    - `int TryAddToInput(InventoryObject item, int amount)` ➔ Gerçekte eklenen miktarı döner (böylece veri kaybı olmaz).
    - `int TryAddToOutput(InventoryObject item, int amount)`
    - `int TryRemoveFromOutput(InventoryObject item, int amount)`
    - `int TryRemoveFromInput(InventoryObject item, int amount)`
    - Transporter için: `TransferToInputOf(WorkerInventory receiver, ItemData item, int maxAmount)`

---

### Aşama 2: İstasyon Entegrasyonları (Mining, Processor, Selling)
> **Amaç**: Duran işçilerin (`WorkMode`) madende kazmasını, fırını beslemesini ve satış yapmasını `WorkerInventory`'ye bağlamak.

- **[MODIFY] [MiningArea.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Areas/MiningArea.cs)**:
  - Üretilen cevheri `WorkerInventory`'nin **Output** slotuna ekleyecek şekilde güncellenmesi.
- **[MODIFY] [ProcessorInputArea.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Building/Processor/ProcessInputArea.cs)**:
  - Sadece `PlayerInventory` kontrolü kaldırılır.
  - `WorkerInventory` geldiğinde, işçinin **Input** slotlarındaki işlenebilir item taranır ve makine kuyruğuna (`AddInput`) beslenir.
- **[MODIFY] [SellingArea.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Areas/SellingArea.cs)**:
  - `WorkerInventory` geldiğinde işçinin **Output** slotundaki satılabilir itemları nakde çevirir.
- **[PREFAB] AutoProcessor Prefab Ayarı**:
  - `InteractionArea` (Giriş) makine görselinin solundaki huniye kaydırılır.
  - `CollectionArea` (Çıkış) makine görselinin sağındaki çıkışa kaydırılır.

---

### Aşama 3: Rota Çizim Sistemi (RouteDrawer)
> **Amaç**: Oyuncunun sol tık ile harita üzerinde taşıyıcı için yol çizebilmesini sağlamak.

- **[NEW] [RouteDrawer.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Worker/RouteDrawer.cs)**:
  - Taşıma modu aktifken fare hareketlerini izler.
  - Sol tık basılı tutulup sürüklendikçe farenin geçtiği grid hücrelerini (`Vector3Int`) listeye ekler.
  - `LineRenderer` kullanarak zemin üzerinde beyaz bir iz çizer.
  - Engelli / yürünemez tile kontrolü (duvar veya boşluk varsa çizgi orada durur).
  - Sağ tık tıklandığında rota çizimini iptal eder.
- **[MODIFY] [WorkerMovement.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Worker/WorkerMovement.cs)**:
  - `SetPatrolRoute(List<Vector3Int> route)` metodu eklenir.
  - İşçi çizilen rotayı 0 ➔ N ➔ 0 şeklinde **Ping-Pong** döngüsüyle sürekli yürür.

---

### Aşama 4: Transporter Lojistik Motoru & Temas Mantığı
> **Amaç**: Transporter'ın rota üstünde yürürken yanından geçtiği işçilerle eşya alışverişi yapması.

- **[MODIFY] [Worker.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Worker/Worker.cs)**:
  - `ItemData TransportItem`: Taşıyıcının taşımakla yükümlü olduğu tek odak item.
  - Durum yönetimi (`WorkerState.Transporting`).
- **[MODIFY] [WorkerInteraction.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Worker/WorkerInteraction.cs)** (veya Transporter lojistik trigger'ı):
  - Transporter yürürken başka bir `Worker` ile temas ettiğinde (Trigger):
    1. **Boşaltma**: Karşıdaki işçinin `Input`'una odak item'ı boşaltır (alabildiği kadar).
    2. **Yükleme**: Karşıdaki işçinin `Output`'unda odak item varsa sırtına yükler (kendi kargo kapasitesi yettiği kadar).
  - İşlem anlık ve akıcı gerçekleşir, transporter yürümesine ara vermez.

---

### Aşama 5: UI & Görsel Geri Bildirim (Paneller ve Ünlem İkonu)
> **Amaç**: Oyuncunun bu sistemi kolayca yönetmesi ve darboğazları görmesi.

- **[MODIFY] [AssignTaskPanelUI.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/UI/WorkerUI/AssignTaskPanelUI.cs)**:
  - `Transport` butonuna tıklandığında taşınacak item'ı belirleme ve `RouteDrawer`'ı tetikleme.
- **[MODIFY] [WorkModePanelUI.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/UI/WorkerUI/WorkModePanelUI.cs)**:
  - İşçinin Input (3 slot) ve Output (3 slot) durumunu gösterecek slot UI desteği.
- **[NEW] [WorkerStatusIcon.cs](file:///Users/bartug/Mining%20Tycoon/Assets/Scripts/Worker/WorkerStatusIcon.cs)**:
  - İşçinin kafasının üstünde beliren `[!]` ikonu.
  - Madenci dolduğunda, Operatör yanlış itemla tıkandığında veya Transporter taşıyacak mal bulamadığında yanıp söner.

---

## 3. Doğrulama ve Test Adımları

1. **Aşama 1 Testi**: `WorkerInventory` unit mantığı; 30 kapasiteli bir işçiye tek üründen 30 eklenebiliyor mu, 3 farklı üründen 10-10-10 paylaştırılabiliyor mu?
2. **Aşama 2 Testi**: Madenci kazıp Output'unu dolduruyor mu? Operatör Input'undaki cevheri arkasındaki fırına atıp pişirebiliyor mu?
3. **Aşama 3 Testi**: Ekranda sol tıkla beyaz hat çizilip sağ tıkla iptal edilebiliyor mu? İşçi o hatta gidip geliyor mu?
4. **Aşama 4 Testi**: Kömür taşıyan transporter, madenciden kömürü alıp fırındaki işçiye teslim ediyor mu? Külçeye dokunmadan geçiyor mu?
5. **Aşama 5 Testi**: Darboğaz anında işçinin kafasında kırmızı ünlem çıkıyor mu?
