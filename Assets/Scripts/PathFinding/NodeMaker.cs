using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Çarpışma Tilemap'ini tarayarak yol bulma ızgarasını (grid) oluşturan ve yöneten bileşendir.
public class NodeMaker : MonoBehaviour
{
    [SerializeField] private Tilemap collisionsTilemap; 

    private readonly Dictionary<Vector3Int, Node> gridNodes = new Dictionary<Vector3Int, Node>();

    // Komşu hücre sorgularında her adımda yeni dizi allocate etmemek için sabit yönler.
    private static readonly Vector3Int[] Directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    private void Awake()
    {
        CreateGrid();
    }

    // Tilemap sınırları içindeki tüm hücreleri dolaşarak yürünebilir ve yürünemez node'ları oluşturur.
    private void CreateGrid()
    {
        if (collisionsTilemap == null)
        {
            Debug.LogError("[NodeMaker] Collisions Tilemap atanmamış!");
            return;
        }

        BoundsInt bounds = collisionsTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                bool walkable = !collisionsTilemap.HasTile(cellPos);

                gridNodes.Add(cellPos, new Node(walkable, cellPos));
            }
        }
        
        Debug.Log($"Harita tarandı! Toplam Node sayısı: {gridNodes.Count}");
    }

    // Belirtilen koordinattaki hücreyi döner, bulunamazsa null döner.
    public Node GetNode(Vector3Int cellPos)
    {
        return gridNodes.TryGetValue(cellPos, out Node node) ? node : null;
    }

    // Bina inşa edildiğinde veya kaldırıldığında hücrenin yürünebilirlik durumunu günceller.
    public void UpdateNodeWalkability(Vector3Int cellPos, bool isWalkable)
    {
        if (gridNodes.TryGetValue(cellPos, out Node node))
        {
            node.isWalkable = isWalkable;
        }
    }

    // Verilen hücrenin 4 tarafındaki (Sağ, Sol, Yukarı, Aşağı) geçerli komşuları döner.
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        if (node == null) return neighbors;

        foreach (Vector3Int dir in Directions)
        {
            Vector3Int checkPos = node.gridPosition + dir;
            if (gridNodes.TryGetValue(checkPos, out Node neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}