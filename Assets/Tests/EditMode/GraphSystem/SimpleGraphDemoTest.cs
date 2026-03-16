using NUnit.Framework;
using Systems.GraphSystem;
using Systems.GraphSystem.Graphs;
using System.Collections.Generic;

namespace Tests.EditMode.GraphSystem
{
    public class SimpleGraphDemoTest
    {
        [Test]
        public void Bfs_ShouldFindAllConnectedMatchingNodes()
        {
            // Setup a simple graph: 1 -- 1 -- 2
            var graph = new IntGraph();
            var node1 = new IntNode(1);
            var node2 = new IntNode(1);
            var node3 = new IntNode(2);

            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node2, node3);

            // Run BFS starting from node1 looking for value 1
            List<IntNode> matchingNodes = IntGraph.FindWantedNodesWithBfs(node1, 1);

            // Assertions
            Assert.AreEqual(2, matchingNodes.Count, "Should find 2 nodes with value 1");
            Assert.Contains(node1, matchingNodes);
            Assert.Contains(node2, matchingNodes);
            Assert.IsFalse(matchingNodes.Contains(node3), "Should NOT find node with value 2");
        }
    }
}
