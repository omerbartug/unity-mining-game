/// <summary>
/// Bünyesinde eşya depolayan ve bu eşyaları bir aktörün envanterine aktarabilen nesnelerin (Binalar, İşçiler) sözleşmesidir.
/// </summary>
public interface IItemSource
{
    /// <summary>
    /// Kaynakta biriken eşyaları hedef envantere aktarır.
    /// </summary>
    /// <param name="targetInventory">Eşyaların aktarılacağı hedef envanter (Oyuncu veya İşçi).</param>
    void CollectItems(Inventory targetInventory);
}

