using UnityEngine;

// Sahneye yerleştirilebilir ve tıklanabilir nesnelerin (Bina, İşçi vb.) girdi ve yerleşim sözleşmesidir.
public interface IObjectInputManager
{
    // Yerleştirme modunun aktif olup olmadığını döner.
    bool IsPlacementMode { get; }

    // Seçili envanter slotunu kontrol ederek yerleştirme modunu günceller.
    void UpdatePlacementMode();

    // Fare sol tıkını işler (yerleştirme veya UI açma).
    void HandleLeftClick(Vector2 mousePosition);

    // Yerleşim onaylandığında nesneyi sahneye kurar.
    void HandlePlaced(Vector3 worldPos, Vector3Int cellPosition);

    // Tıklanan noktadaki nesnenin UI panelini açmayı dener.
    void TryOpenUI(Vector2 mousePosition);

    // Yerleştirme modunu iptal eder ve hayaleti gizler.
    void CancelPlacementMode();
}
