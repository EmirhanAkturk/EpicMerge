using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;

namespace Utils.Extensions
{
    public static class TileGraphExtensions 
    {
        public static void FindEdgesWithNodeDistance(this TileGraph graph , float nodeDistance/*, bool crateVertexObject = false*/) 
        {
            var nodes = graph.GetNodes();
            
            foreach (var node1 in nodes)
            {
                foreach (var node2 in nodes)
                {
                    if(node1 == node2) continue;
                    if (Vector3.Distance(node1.Transform.position, node2.Transform.position) <= nodeDistance)
                    {
                        graph.AddEdge(node1, node2);
                        // if (crateVertexObject)
                        // {
                        //     node1.CreateVertexObject(node2);
                        // }
                    }
                }
            }
        }
        
        public static void DestroyImmediateNodes(this TileGraph tileGraph)
        {
            var nodes = tileGraph.GetNodes();

            foreach (var node in nodes)
            {
                Object.DestroyImmediate(node.Transform.gameObject);
            }
     
            tileGraph.ClearNodes();
        }        
        
        public static void DestroyNodes(this TileGraph tileGraph)
        {
            var nodes = tileGraph.GetNodes();

            foreach (var node in nodes)
            {
                Object.Destroy(node.Transform.gameObject);
            }
     
            tileGraph.ClearNodes();
        }
    }
}
