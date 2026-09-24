using System.Collections.Generic;
using UnityEngine;

// İki ızgara hücresi arasında A* algoritmasını kullanarak en kısa yolu hesaplayan bileşendir.
public class Pathfinding : MonoBehaviour
{
    private NodeMaker grid;

    private void Awake()
    {
        grid = GetComponent<NodeMaker>();
    }

    // Başlangıç ve hedef koordinatları arasında A* ile en kısa rotayı bulur.
    public List<Node> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        Node startNode = grid.GetNode(startPos);
        Node targetNode = grid.GetNode(targetPos);

        // Hedef yoksa veya yürünemez bir engelse (duvar/bina) arama yapma
        if (startNode == null || targetNode == null || !targetNode.isWalkable)
        {
            Debug.Log("Hedef geçersiz veya duvar!");
            return null; 
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);  

        // İncelenecek aday hücreler bitene kadar en uygun hücreyi genişlet
        while (openSet.Count > 0)
        {
            Node currentNode = GetBestNode(openSet);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Hedefe ulaşıldıysa parent zincirini takip ederek rotayı oluştur
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            CheckNeighbors(currentNode, targetNode, openSet, closedSet);
        }

        return new List<Node>();
    }

    // Hedef hücreden başlayarak ebeveyn (parent) zincirini geriye doğru takip edip rotayı sıraya dizer.
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode); 
            currentNode = currentNode.parent;
        }
        
        path.Reverse(); // Başlangıçtan hedefe doğru sırala
        return path;
    }

    // İki hücre arasındaki Manhattan mesafesini (|dx| + |dy|) hesaplar (4 yönlü ızgara sezgiseli).
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        return (dstX + dstY);
    }

    // Açık kümedeki en düşük FCost'a (ve eşitlik durumunda en düşük hCost'a) sahip hücreyi seçer.
    private Node GetBestNode(List<Node> openSet)
    {
        Node currentNode = openSet[0];
            
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].FCost < currentNode.FCost || 
                (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
            {
                currentNode = openSet[i];
            }
        }

        return currentNode;
    }

    // Mevcut hücrenin komşularını inceler; daha ucuz rota varsa maliyetleri güncelleyip openSet'e ekler.
    private void CheckNeighbors(Node currentNode, Node targetNode, List<Node> openSet, HashSet<Node> closedSet)
    {
        foreach (Node neighbor in grid.GetNeighbors(currentNode))
        {
            // Yürünemez hücreleri ve daha önce incelenmişleri atla
            if (!neighbor.isWalkable || closedSet.Contains(neighbor))
            {
                continue;
            }

            int newCostToNeighbor = currentNode.gCost + 1;

            // Bu komşuya ilk defa geliniyorsa veya daha ucuz bir yol bulunduysa güncelle
            if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
            {
                neighbor.gCost = newCostToNeighbor;
                neighbor.hCost = GetDistance(neighbor, targetNode);
                neighbor.parent = currentNode;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
            }
        }
    }
}