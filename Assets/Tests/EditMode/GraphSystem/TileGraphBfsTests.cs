using NUnit.Framework;
using UnityEngine;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;

namespace Tests.EditMode.GraphSystem
{
    /// <summary>
    /// TileGraph + TileNode BFS testleri.
    /// Merge detection'ın temelini oluşturan connected component bulma senaryoları test edilir.
    /// </summary>
    public class TileGraphBfsTests
    {
        private TileGraph _graph;

        private static readonly TileObjectValue Empty = TileObjectValue.GetEmptyTileObjectValue();
        private static readonly TileObjectValue TypeA1 = new TileObjectValue(1, 1);
        private static readonly TileObjectValue TypeA2 = new TileObjectValue(1, 2); // farklı level
        private static readonly TileObjectValue TypeB1 = new TileObjectValue(2, 1); // farklı id

        [SetUp]
        public void SetUp()
        {
            _graph = new TileGraph();
        }

        // ─── Temel merge tespiti ──────────────────────────────────────────────

        [Test]
        public void Bfs_ThreeConnectedSameTiles_FindsAllThree()
        {
            // Merge için gereken minimum 3 tile senaryosu:
            // A - B - C (hepsi TypeA1)
            var a = AddNode("A", TypeA1);
            var b = AddNode("B", TypeA1);
            var c = AddNode("C", TypeA1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(b, c);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.AreEqual(3, result.Count);
            Assert.Contains(a, result);
            Assert.Contains(b, result);
            Assert.Contains(c, result);
        }

        [Test]
        public void Bfs_TwoConnectedSameTiles_CannotMerge()
        {
            // 2 tile: merge için yetmez, ama BFS doğru sayı döndürmeli
            var a = AddNode("A", TypeA1);
            var b = AddNode("B", TypeA1);

            _graph.AddEdge(a, b);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Bfs_EmptyTileInBetween_BlocksMerge()
        {
            // A(TypeA1) - E(Empty) - B(TypeA1)
            // Empty node TypeA1 ile eşleşmediği için geçit vermez
            var a = AddNode("A", TypeA1);
            var empty = AddNode("E", Empty);
            var b = AddNode("B", TypeA1);

            _graph.AddEdge(a, empty);
            _graph.AddEdge(empty, b);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result.Contains(b));
        }

        [Test]
        public void Bfs_DifferentTypeTileInBetween_BlocksMerge()
        {
            // A(TypeA1) - X(TypeB1) - B(TypeA1)
            var a = AddNode("A", TypeA1);
            var x = AddNode("X", TypeB1);
            var b = AddNode("B", TypeA1);

            _graph.AddEdge(a, x);
            _graph.AddEdge(x, b);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result.Contains(b));
        }

        [Test]
        public void Bfs_DifferentLevelSameTile_NotMergeable()
        {
            // TypeA1 ve TypeA2 aynı id ama farklı level → eşleşmez
            var a = AddNode("A", TypeA1);
            var b = AddNode("B", TypeA2);

            _graph.AddEdge(a, b);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result.Contains(b));
        }

        // ─── Grid senaryoları ─────────────────────────────────────────────────

        [Test]
        public void Bfs_2x2Grid_AllSameType_FindsAll()
        {
            // [A][B]
            // [C][D]  hepsi TypeA1
            var a = AddNode("A", TypeA1);
            var b = AddNode("B", TypeA1);
            var c = AddNode("C", TypeA1);
            var d = AddNode("D", TypeA1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(a, c);
            _graph.AddEdge(b, d);
            _graph.AddEdge(c, d);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.AreEqual(4, result.Count);
        }

        [Test]
        public void Bfs_2x2Grid_MixedTypes_FindsOnlyMatchingComponent()
        {
            // [A1][B1]
            // [C2][D1]  → B1 ve D1 arasındaki bağlantı C2'den geçmez
            //
            // Komşuluk:
            //   A1-B1, A1-C2, B1-D1, C2-D1
            // TypeA1 BFS'i (A'dan): A→B→D bulur, C'den geçemez
            var a = AddNode("A", TypeA1);
            var b = AddNode("B", TypeA1);
            var c = AddNode("C", TypeB1);
            var d = AddNode("D", TypeA1);

            _graph.AddEdge(a, b);
            _graph.AddEdge(a, c);
            _graph.AddEdge(b, d);
            _graph.AddEdge(c, d);

            var result = TileGraph.FindWantedNodesWithBfs(a, TypeA1);

            Assert.IsTrue(result.Contains(a));
            Assert.IsTrue(result.Contains(b));
            Assert.IsTrue(result.Contains(d));
            Assert.IsFalse(result.Contains(c));
        }

        // ─── except parametresi: drag-drop senaryosu ─────────────────────────

        [Test]
        public void Bfs_ExceptDraggedNode_ExcludesItFromSearch()
        {
            // Kullanıcı A'yı sürüklüyor; A hariç B-C'nin merge sayısı kontrol ediliyor
            // B - A(dragged) - C  → A except → B ve C birbirinden kopuk → her biri 1
            var a = AddNode("A", TypeA1); // dragged
            var b = AddNode("B", TypeA1);
            var c = AddNode("C", TypeA1);

            _graph.AddEdge(b, a);
            _graph.AddEdge(a, c);

            // B'den başla, A'yı except al → C'ye ulaşılamaz
            var result = TileGraph.FindWantedNodesWithBfs(b, TypeA1, except: a);

            Assert.AreEqual(1, result.Count);
            Assert.Contains(b, result);
            Assert.IsFalse(result.Contains(c));
        }

        // ─── Empty tile operasyonları ─────────────────────────────────────────

        [Test]
        public void Bfs_StartOnEmptyNode_ReturnsOnlyStart()
        {
            var emptyStart = AddNode("E", Empty);
            var b = AddNode("B", TypeA1);

            _graph.AddEdge(emptyStart, b);

            var result = TileGraph.FindWantedNodesWithBfs(emptyStart, Empty);

            // BFS yalnızca empty.Value eşleşen komşulara gider → b eşleşmez
            Assert.AreEqual(1, result.Count);
            Assert.Contains(emptyStart, result);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private TileNode AddNode(string name, TileObjectValue value)
        {
            var go = new GameObject(name);
            var node = new TileNode(go.transform, value);
            _graph.AddNode(node);
            return node;
        }
    }
}