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
                node.PrintNeighborsValues();
            }    
        }
    }
}
