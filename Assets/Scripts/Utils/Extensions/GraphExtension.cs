using System.Collections.Generic;
using Systems.GraphSystem;
using UnityEngine;

namespace Utils.Extensions
{
    public static class GraphExtension 
    {
        public static void PrintGraphNeighbors(this Graph graph)
        {
            var nodes = graph.GetNodes();
            foreach (var node in nodes)
            {
                node.PrintNeighbors();
            }    
        }
        
        public static void FindEdgesWithNodeDistance(this Graph graph, float nodeDistance, bool printNeighbors = false)
        {
            var nodes = graph.GetNodes();
            
            foreach (var node1 in nodes)
            {
                foreach (var node2 in nodes)
                {
                    if(node1 == node2) continue;
                    if (Vector3.Distance(node1.transform.position, node2.transform.position) <= nodeDistance)
                    {
                        graph.AddEdge(node1, node2);
                        node1.CreateVertexObject(node2);
                    }
                }
            }

            if (printNeighbors)
            {
                graph.PrintGraphNeighbors();
            }
        }

        public static void DestroyImmediateNodes(this Graph graph)
        {
            var nodes = graph.GetNodes();

            foreach (var node in nodes)
            {
                Object.DestroyImmediate(node.gameObject);
            }
     
            graph.ClearNodes();
        }        
        
        public static void DestroyNodes(this Graph graph)
        {
            var nodes = graph.GetNodes();

            foreach (var node in nodes)
            {
                Object.Destroy(node.gameObject);
            }
     
            graph.ClearNodes();
        }
    }
}
