using UnityEngine;

/// <summary>
/// Oyuncu veya işçiler tarafından zaman harcanarak etkileşime girilebilen tüm dünya nesnelerinin (Madenler, Fırınlar, Konteynerler) sözleşmesidir.
/// Yaşam döngüsü: TryGetInteractionData (Doğrula) -> OperationTime (Süreç) -> CompleteInteract (Tamamla) veya CancelInteract (İptal).
/// </summary>
public interface IInteractable
{
    
    // Etkilesim sirasinda isci ne is yapiyor onu tutar (Madencilik, İşleme, Operating vb.).
    WorkerWorkType WorkType { get; }

    
    // Etkileşimin kaç saniye süreceğini belirtir.
    float OperationTime { get; }

    /// <summary>
    /// Etkileşimin başlatılıp başlatılamayacağını doğrular ve gerekli eşya/miktar verisini döner.
    /// </summary>
    /// <param name="inventory">Etkileşime giren aktörün envanteri.</param>
    /// <param name="item">İşlemde kullanılacak veya üretilecek eşya.</param>
    /// <param name="amount">İşlem miktarı.</param>
    /// <returns>Etkileşim şartları sağlanıyorsa true, aksi halde false.</returns>
    bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount);

    
    // Etkileşim süresi başarıyla tamamlandığında çalıştırılır.
    void CompleteInteract(Inventory inventory, ItemData item, int amount);


    /// Etkileşim süresi dolmadan tuş bırakılırsa veya alandan çıkılırsa çalıştırılır.
    /// <param name="progress">Sıfırlanacak ilerleme çubuğu UI referansı.</param>
    void CancelInteract(ProgressBar progress);
}

