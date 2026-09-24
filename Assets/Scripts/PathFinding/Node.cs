using UnityEngine;

// Izgara tabanlı haritadaki tek bir hücreyi temsil eden saf veri sınıfıdır.
public class Node 
{
    // Hücreden yürünerek geçilip geçilemeyeceğini belirtir (çarpışma ve bina kontrolü).
    public bool isWalkable;

    // Hücrenin Tilemap üzerindeki ızgara (grid) koordinatıdır.
    public Vector3Int gridPosition;

    // Başlangıç noktasından bu hücreye kadar olan adım maliyeti (G Cost).
    public int gCost;

    // Bu hücreden hedefe olan tahmini sezgisel mesafe maliyeti (H Cost).
    public int hCost;

    // Toplam maliyet (F = G + H). A* algoritmasında öncelik sırasını belirler.
    public int FCost 
    {
        get { return gCost + hCost; }
    }

    // Yol geriye doğru takip edilirken (backtracking) bu hücreye nereden gelindiğini tutar.
    public Node parent;

    public Node(bool _isWalkable, Vector3Int _gridPosition)
    {
        isWalkable = _isWalkable;
        gridPosition = _gridPosition;
    }
}