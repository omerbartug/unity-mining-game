
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
