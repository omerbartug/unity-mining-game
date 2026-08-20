using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private NodeMaker grid;

    private void Awake()
    {
        grid = GetComponent<NodeMaker>();
    }

    public List<Node> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        Node startNode = grid.GetNode(startPos);
        Node targetNode = grid.GetNode(targetPos);

        
        if (startNode == null || targetNode == null || !targetNode.isWalkable)
        {
            Debug.Log("Hedef geçersiz veya duvar!");
            return null; 
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

 
        openSet.Add(startNode);  
        while (openSet.Count > 0)
        {
            Node currentNode = GetBestNode(openSet);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode); // getting path
            }

            CheckNeighbors(currentNode, targetNode, openSet,  closedSet);
        }

        return new List<Node>();
    }


//HELPER METHODS
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        
        while (currentNode != startNode)
        {
            path.Add(currentNode); 
            currentNode = currentNode.parent;
        }
        
        path.Reverse(); // reversing start to end

        return path;
    }
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        return (dstX + dstY);
    }
    private Node GetBestNode(List<Node> openSet)
    {
        Node currentNode = openSet[0]; // baslangic noktasi
            
        for (int i = 1; i < openSet.Count; i++)
        {
            //defterdeki f i iyi olan node u sec
            if (openSet[i].FCost < currentNode.FCost || 
                (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
            {
                currentNode = openSet[i];
            }
        }

        return currentNode;

    }
    private void CheckNeighbors(Node currentNode, Node targetNode, List<Node> openSet, HashSet<Node> closedSet)
    {
            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                
                int newCostToNeighbor = currentNode.gCost + 1;

                
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // update node's f data
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