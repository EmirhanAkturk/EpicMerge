using NUnit.Framework;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;

namespace Tests.EditMode.GraphSystem
{
    /// <summary>
    /// TileObjectValue struct testleri.
    /// Pure C# struct, Unity dependency'si yok.
    /// </summary>
    public class TileObjectValueTests
    {
        // ─── Equals ──────────────────────────────────────────────────────────

        [Test]
        public void Equals_SameIdAndLevel_ReturnsTrue()
        {
            var a = new TileObjectValue(1, 2);
            var b = new TileObjectValue(1, 2);

            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_DifferentId_ReturnsFalse()
        {
            var a = new TileObjectValue(1, 2);
            var b = new TileObjectValue(9, 2);

            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_DifferentLevel_ReturnsFalse()
        {
            var a = new TileObjectValue(1, 2);
            var b = new TileObjectValue(1, 9);

            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_BoxedObject_ReturnsTrue()
        {
            var a = new TileObjectValue(3, 5);
            object boxed = new TileObjectValue(3, 5);

            Assert.IsTrue(a.Equals(boxed));
        }

        [Test]
        public void Equals_BoxedWrongType_ReturnsFalse()
        {
            var a = new TileObjectValue(1, 1);

            Assert.IsFalse(a.Equals("not a TileObjectValue"));
        }

        // ─── CopyConstructor ─────────────────────────────────────────────────

        [Test]
        public void CopyConstructor_CreatesEqualValue()
        {
            var original = new TileObjectValue(7, 3);
            var copy = new TileObjectValue(original);

            Assert.IsTrue(original.Equals(copy));
            Assert.AreEqual(original.objectId, copy.objectId);
            Assert.AreEqual(original.objectLevel, copy.objectLevel);
        }

        // ─── Empty Value ──────────────────────────────────────────────────────

        [Test]
        public void IsEmptyTileObjectValue_EmptyValue_ReturnsTrue()
        {
            var empty = TileObjectValue.GetEmptyTileObjectValue();

            Assert.IsTrue(empty.IsEmptyTileObjectValue());
        }

        [Test]
        public void IsEmptyTileObjectValue_NonEmptyValue_ReturnsFalse()
        {
            var value = new TileObjectValue(1, 1);

            Assert.IsFalse(value.IsEmptyTileObjectValue());
        }

        [Test]
        public void GetEmptyTileObjectValue_HasIdMinusOne()
        {
            var empty = TileObjectValue.GetEmptyTileObjectValue();

            Assert.AreEqual(-1, empty.objectId);
            Assert.AreEqual(0, empty.objectLevel);
        }

        [Test]
        public void GetEmptyTileObjectValue_MultipleCallsReturnEqualValues()
        {
            var e1 = TileObjectValue.GetEmptyTileObjectValue();
            var e2 = TileObjectValue.GetEmptyTileObjectValue();

            Assert.IsTrue(e1.Equals(e2));
        }

        // ─── GetHashCode ─────────────────────────────────────────────────────

        [Test]
        public void GetHashCode_SameValues_SameHash()
        {
            var a = new TileObjectValue(4, 2);
            var b = new TileObjectValue(4, 2);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentValues_DifferentHash()
        {
            var a = new TileObjectValue(1, 1);
            var b = new TileObjectValue(2, 1);

            // Hash collision olabilir ama tipik değerler için eşit olmaz
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}