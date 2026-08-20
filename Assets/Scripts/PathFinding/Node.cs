using UnityEngine;

// MonoBehaviour yok cünkü bu sahnede duran bir obje değil, sadece saf veri.
// Saf C# sınıfları olduğu için binlerce Node olsa bile oyun kasmayacak.
public class Node 
{
    public bool isWalkable; // Bu kareden geçilir mi? (Collisions kontrolü için)
    public Vector3Int gridPosition; // Karenin koordinatı
    

    // Yol bulma (Pathfinding) maliyetleri
    public int gCost; 
    public int hCost; 
    // F Cost her zaman G ve H'nin toplamıdır, o yüzden sadece sorulduğunda hesaplanıp verilir
    public int FCost 
    {
        get { return gCost + hCost; }
    }
    

    // Geriye dönük yolu (Backtracking) çizebilmek için geldiğimiz kareyi tutuyoruz
    public Node parent; 


    // Constructor (Bu node'u oluştururken bilgileri içine atıyoruz)
    public Node(bool _isWalkable, Vector3Int _gridPosition)
    {
        isWalkable = _isWalkable;
        gridPosition = _gridPosition;
    }

    
}