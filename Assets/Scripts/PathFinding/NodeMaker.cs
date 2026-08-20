using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeMaker : MonoBehaviour
{
    [SerializeField] private Tilemap collisionsTilemap; 
    

    public Dictionary<Vector3Int, Node> gridNodes = new Dictionary<Vector3Int, Node>();

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
       
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

   
    public Node GetNode(Vector3Int cellPos)
    {
        if (gridNodes.ContainsKey(cellPos))
        {
            return gridNodes[cellPos];
        }
        
        return null;
    }

    public void UpdateNodeWalkability(Vector3Int cellPos, bool isWalkable)
    {
        if (gridNodes.ContainsKey(cellPos))
        {
            gridNodes[cellPos].isWalkable = isWalkable;
        }
    }

    
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),  
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),  
            new Vector3Int(0, -1, 0)  
        };

       
        foreach (Vector3Int dir in directions)
        {
            Vector3Int checkPos = node.gridPosition + dir;

            if (gridNodes.ContainsKey(checkPos))
            {
                neighbors.Add(gridNodes[checkPos]);
            }
        }

        return neighbors;
    }
}