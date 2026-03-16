using System.Collections.Generic;
using NUnit.Framework;
using Systems.GraphSystem;
using Systems.GraphSystem.Graphs;

namespace Tests.EditMode.GraphSystem
{
    /// <summary>
    /// Graph<T,TF> generic logic testleri.
    /// IntGraph / IntNode kullanarak Unity dependency'si olmadan test edilir.
    /// </summary>
    public class GraphTests
    {
        private IntGraph _graph;

        [SetUp]
        public void SetUp()
        {
            _graph = new IntGraph();
        }

        // ─── AddNode / GetNodes ───────────────────────────────────────────────

        [Test]
        public void AddNode_SingleNode_GraphContainsIt()
        {
            var node = new IntNode(1);
            _graph.AddNode(node);

            Assert.Contains(node, _graph.GetNodes());
        }

        [Test]
        public void AddNode_MultipleNodes_AllPresent()
        {
            var a = new IntNode(1);
            var b = new IntNode(2);
            var c = new IntNode(3);

            _graph.AddNode(a);
            _graph.AddNode(b);
            _graph.AddNode(c);

            var nodes = _graph.GetNodes();
            Assert.AreEqual(3, nodes.Count);
            Assert.Contains(a, nodes);
            Assert.Contains(b, nodes);
            Assert.Contains(c, nodes);
        }

        // ─── AddEdge ─────────────────────────────────────────────────────────

        [Test]
        public void AddEdge_CreatesBidirectionalConnection()
        {
            var a = new IntNode(1);
            var b = new IntNode(2);

            _graph.AddEdge(a, b);

            Assert.IsTrue(a.IsNeighbor(b));
            Assert.IsTrue(b.IsNeighbor(a));
        }

        [Test]
        public void AddEdge_SameNode_DoesNotCreateSelfLoop()
        {
            var a = new IntNode(1);

            _graph.AddEdge(a, a);

            Assert.IsFalse(a.IsNeighbor(a));
            Assert.AreEqual(0, a.GetNeighbors().Count);
        }

        [Test]
        public void AddEdge_Duplicate_NotAddedTwice()
        {
            var a = new IntNode(1);
            var b = new IntNode(1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(a, b); // tekrar ekle

            Assert.AreEqual(1, a.GetNeighbors().Count);
            Assert.AreEqual(1, b.GetNeighbors().Count);
        }

        // ─── ClearNodes ──────────────────────────────────────────────────────

        [Test]
        public void ClearNodes_RemovesAllNodes()
        {
            _graph.AddNode(new IntNode(1));
            _graph.AddNode(new IntNode(2));

            _graph.ClearNodes();

            Assert.AreEqual(0, _graph.GetNodes().Count);
        }

        // ─── BFS: Temel Durumlar ─────────────────────────────────────────────

        [Test]
        public void Bfs_SingleNode_ReturnsSelf()
        {
            var a = new IntNode(5);

            var result = IntGraph.FindWantedNodesWithBfs(a, 5);

            Assert.AreEqual(1, result.Count);
            Assert.Contains(a, result);
        }

        [Test]
        public void Bfs_StartNodeNotMatchingTarget_ReturnsEmpty()
        {
            // start.Value == 1, targetValue == 99 → hiçbir node eşleşmez
            var a = new IntNode(1);
            var b = new IntNode(1);
            _graph.AddEdge(a, b);

            var result = IntGraph.FindWantedNodesWithBfs(a, 99);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Bfs_ConnectedMatchingNodes_ReturnsAll()
        {
            // A-B-C hepsi value=1, hepsi birbirine bağlı
            var a = new IntNode(1);
            var b = new IntNode(1);
            var c = new IntNode(1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(b, c);

            var result = IntGraph.FindWantedNodesWithBfs(a, 1);

            Assert.AreEqual(3, result.Count);
            Assert.Contains(a, result);
            Assert.Contains(b, result);
            Assert.Contains(c, result);
        }

        [Test]
        public void Bfs_DifferentValueBlocks_StopsAtBoundary()
        {
            // A(1) - B(2) - C(1)  →  B(2) geçit vermez, C bulunmamalı
            var a = new IntNode(1);
            var b = new IntNode(2);
            var c = new IntNode(1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(b, c);

            var result = IntGraph.FindWantedNodesWithBfs(a, 1);

            Assert.AreEqual(1, result.Count);
            Assert.Contains(a, result);
            Assert.IsFalse(result.Contains(c));
        }

        [Test]
        public void Bfs_DisconnectedMatchingNodes_OnlyConnectedComponentReturned()
        {
            // A(1) - B(1)   C(1) (bağlantısız)
            var a = new IntNode(1);
            var b = new IntNode(1);
            var c = new IntNode(1); // bağlantısız

            _graph.AddEdge(a, b);

            var result = IntGraph.FindWantedNodesWithBfs(a, 1);

            Assert.AreEqual(2, result.Count);
            Assert.Contains(a, result);
            Assert.Contains(b, result);
            Assert.IsFalse(result.Contains(c));
        }

        // ─── BFS: except parametresi ─────────────────────────────────────────

        [Test]
        public void Bfs_ExceptNode_IsSkippedDuringTraversal()
        {
            // A(1) - B(1) - C(1)  →  B except olunca C'ye ulaşılamaz
            var a = new IntNode(1);
            var b = new IntNode(1);
            var c = new IntNode(1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(b, c);

            var result = IntGraph.FindWantedNodesWithBfs(a, 1, except: b);

            Assert.IsFalse(result.Contains(b));
            Assert.IsFalse(result.Contains(c));
        }

        [Test]
        public void Bfs_ExceptNode_DoesNotBlockAlternatePath()
        {
            // A(1) - B(1), A(1) - C(1) - B(1)  →  B except ama C üzerinden ulaşılabilir
            var a = new IntNode(1);
            var b = new IntNode(1);
            var c = new IntNode(1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(a, c);
            _graph.AddEdge(c, b);

            var result = IntGraph.FindWantedNodesWithBfs(a, 1, except: b);

            Assert.IsTrue(result.Contains(a));
            Assert.IsTrue(result.Contains(c));
            Assert.IsFalse(result.Contains(b));
        }

        // ─── BFS: Farklı targetValue senaryosu ───────────────────────────────

        [Test]
        public void Bfs_TargetValueDifferentFromStartValue_ReturnsEmpty()
        {
            // BFS sadece start.Value eşleşen komşulara gider;
            // targetValue farklıysa hiçbir node bulunamaz.
            var a = new IntNode(1);
            var b = new IntNode(2);

            _graph.AddEdge(a, b);

            // start=a(value=1), targetValue=2
            // b(value=2) ≠ start.Value(1) → queue'ya eklenmez → bulunamaz
            var result = IntGraph.FindWantedNodesWithBfs(a, 2);

            Assert.AreEqual(0, result.Count);
        }
    }
}