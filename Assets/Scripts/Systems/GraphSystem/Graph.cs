using System.Collections.Generic;
using UnityEngine;

namespace Systems.GraphSystem
{
    /// <typeparam name="T"> Neighbor Node Type </typeparam>
    /// <typeparam name="TF"> Node Value Type </typeparam>
    public class Graph<T, TF> where T : Node<T, TF>
    {
        private readonly List<T> nodes;

        protected Graph()
        {
            nodes = new List<T>();
        }

        public void AddNode(T node)
        {
            nodes.Add(node);
        }

        public void AddEdge(T node1, T node2)
        {
            if(node1 == node2) return;
            
            node1.AddNeighbor(node2);
            node2.AddNeighbor(node1);
        }

        public List<T> GetNodes()
        {
            return nodes;
        }

        public void ClearNodes()
        {
            nodes.Clear();
        }

        #region Search Functions

        private static readonly HashSet<T> Visited = new HashSet<T>();
        private static readonly Queue<T> Queue = new Queue<T>();

        public static List<T> FindWantedNodesWithBfs(T start, TF targetValue, T except = null)
        {
            Visited.Clear();
            Queue.Clear();

            List<T> results = new List<T>();
            
            Queue.Enqueue(start);
            Visited.Add(start);

            while (Queue.Count > 0)
            {
                var currentNode = Queue.Dequeue();
                if (currentNode.Value.Equals(targetValue))
                {
                    results.Add(currentNode);
                }

                var neighbors = currentNode.GetNeighbors(); 
                foreach (var node in neighbors)
                {
                    var neighbor = node;
                    if(except != null && neighbor == except) continue;
                    
                    if (!Visited.Contains(neighbor) && neighbor.Value.Equals(start.Value))
                    {
                        Queue.Enqueue(neighbor);
                        Visited.Add(neighbor);
                    }
                }
            }

            return results;
        }
            
        #endregion
    }
}