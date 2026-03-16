using NUnit.Framework;
using Systems.GraphSystem.Graphs;
using UnityEngine;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;

namespace Tests.EditMode.GraphSystem
{
    /// <summary>
    /// Node<T,TF> generic logic testleri.
    /// IntNode ile generic davranış, TileNode ile max-neighbor ve GetEmptyNeighbor test edilir.
    /// </summary>
    public class NodeTests
    {
        // ─── Constructor / Value ──────────────────────────────────────────────

        [Test]
        public void Constructor_SetsValue()
        {
            var node = new IntNode(42);

            Assert.AreEqual(42, node.Value);
        }

        [Test]
        public void SetValue_UpdatesValue()
        {
            var node = new IntNode(1);
            node.SetValue(99);

            Assert.AreEqual(99, node.Value);
        }

        // ─── AddNeighbor / IsNeighbor ─────────────────────────────────────────

        [Test]
        public void AddNeighbor_ReturnsTrue_AndNeighborIsAdded()
        {
            var a = new IntNode(1);
            var b = new IntNode(2);

            bool added = a.AddNeighbor(b);

            Assert.IsTrue(added);
            Assert.IsTrue(a.IsNeighbor(b));
        }

        [Test]
        public void AddNeighbor_Duplicate_ReturnsFalse_AndNotAddedTwice()
        {
            var a = new IntNode(1);
            var b = new IntNode(2);

            a.AddNeighbor(b);
            bool addedAgain = a.AddNeighbor(b);

            Assert.IsFalse(addedAgain);
            Assert.AreEqual(1, a.GetNeighbors().Count);
        }

        [Test]
        public void IsNeighbor_NonNeighbor_ReturnsFalse()
        {
            var a = new IntNode(1);
            var b = new IntNode(2);

            Assert.IsFalse(a.IsNeighbor(b));
        }

        // ─── GetNeighbors ─────────────────────────────────────────────────────

        [Test]
        public void GetNeighbors_InitiallyEmpty()
        {
            var node = new IntNode(1);

            Assert.AreEqual(0, node.GetNeighbors().Count);
        }

        [Test]
        public void GetNeighbors_ReturnsAllAddedNeighbors()
        {
            var a = new IntNode(1);
            var b = new IntNode(2);
            var c = new IntNode(3);

            a.AddNeighbor(b);
            a.AddNeighbor(c);

            var neighbors = a.GetNeighbors();
            Assert.AreEqual(2, neighbors.Count);
            Assert.Contains(b, neighbors);
            Assert.Contains(c, neighbors);
        }

        // ─── TileNode: Maksimum komşu sınırı (4) ─────────────────────────────

        [Test]
        public void TileNode_MaxNeighbors_FourNeighborsAccepted()
        {
            var center = MakeTileNode(0);

            center.AddNeighbor(MakeTileNode(1));
            center.AddNeighbor(MakeTileNode(2));
            center.AddNeighbor(MakeTileNode(3));
            bool fourthAdded = center.AddNeighbor(MakeTileNode(4));

            Assert.IsTrue(fourthAdded);
            Assert.AreEqual(4, center.GetNeighbors().Count);
        }

        [Test]
        public void TileNode_MaxNeighbors_FifthNeighborRejected()
        {
            var center = MakeTileNode(0);

            center.AddNeighbor(MakeTileNode(1));
            center.AddNeighbor(MakeTileNode(2));
            center.AddNeighbor(MakeTileNode(3));
            center.AddNeighbor(MakeTileNode(4));
            bool fifthAdded = center.AddNeighbor(MakeTileNode(5));

            Assert.IsFalse(fifthAdded);
            Assert.AreEqual(4, center.GetNeighbors().Count);
        }

        // ─── TileNode: GetEmptyNeighbor ───────────────────────────────────────

        [Test]
        public void TileNode_GetEmptyNeighbor_ReturnsEmptyNeighbor()
        {
            var center = MakeTileNode(0, new TileObjectValue(1, 1));
            var emptyNeighbor = MakeTileNode(1, TileObjectValue.GetEmptyTileObjectValue());

            center.AddNeighbor(emptyNeighbor);

            Assert.AreEqual(emptyNeighbor, center.GetEmptyNeighbor());
        }

        [Test]
        public void TileNode_GetEmptyNeighbor_NoEmptyNeighbor_ReturnsNull()
        {
            var filled = new TileObjectValue(1, 1);
            var center = MakeTileNode(0, filled);
            center.AddNeighbor(MakeTileNode(1, filled));

            Assert.IsNull(center.GetEmptyNeighbor());
        }

        [Test]
        public void TileNode_GetEmptyNeighbor_NoNeighbors_ReturnsNull()
        {
            var center = MakeTileNode(0, new TileObjectValue(1, 1));

            Assert.IsNull(center.GetEmptyNeighbor());
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private TileNode MakeTileNode(int index, TileObjectValue value = default)
        {
            var go = new GameObject($"Node_{index}");
            return new TileNode(go.transform, value);
        }
    }
}