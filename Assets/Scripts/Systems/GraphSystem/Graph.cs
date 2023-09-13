using System.Collections.Generic;

namespace Systems.GraphSystem
{
    public class Graph
    {
        private readonly List<Node> nodes;

        public Graph()
        {
            nodes = new List<Node>();
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void AddEdge(Node node1, Node node2)
        {
            if(node1 == node2) return;
            
            node1.AddNeighbor(node2);
            node2.AddNeighbor(node1);
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }
    }
}