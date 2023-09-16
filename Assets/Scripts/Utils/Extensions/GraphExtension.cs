using System.Collections.Generic;
using Systems.GraphSystem;
using UnityEngine;

namespace Utils.Extensions
{
    public static class GraphExtension 
    {
        public static void PrintGraphNeighbors<T, TF>(this Graph<T, TF> graph) where T : Node<T, TF>
        {
            var nodes = graph.GetNodes();
            foreach (var node in nodes)
            {
                node.PrintNeighbors();
            }    
        }
        
        public static void FindEdgesWithNodeDistance<T, TF>(this Graph<T, TF> graph , float nodeDistance, bool crateVertexObject = false) where T : Node<T, TF> 
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
                        if (crateVertexObject)
                        {
                            node1.CreateVertexObject(node2);
                        }
                    }
                }
            }
        }

        public static void DestroyImmediateNodes<T, TF>(this Graph<T, TF> graph) where T : Node<T, TF>
        {
            var nodes = graph.GetNodes();

            foreach (var node in nodes)
            {
                Object.DestroyImmediate(node.gameObject);
            }
     
            graph.ClearNodes();
        }        
        
        public static void DestroyNodes<T, TF>(this Graph<T, TF> graph) where T : Node<T, TF>
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
