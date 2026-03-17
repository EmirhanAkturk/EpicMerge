using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using _Game.Scripts.Systems.TileSystem.EventSystem;
using _Game.Scripts.Systems.TileSystem.TileMergeSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;

namespace Tests.PlayMode.TileSystem
{
    /// <summary>
    /// TileObjectMergeHelper — görsel + fonksiyonel PlayMode testleri.
    ///
    /// Renk şeması:
    ///   Level 1 (TypeA1) → Mavi
    ///   Level 2 (TypeA2) → Cyan   [upgrade sonucu]
    ///   Level 3 (TypeA3) → Yeşil  [çift upgrade]
    ///   TypeB1           → Turuncu
    ///   Empty            → Gri (küçülür)
    ///   Indicator açık   → tile 1.2x büyür
    ///
    /// Merge dağılımı (MergeRequiredObject = 3):
    ///   3 tile  → 1 upgraded + 2 empty
    ///   4 tile  → 1 upgraded + 1 same + 2 empty
    ///   5 tile  → 2 upgraded + 3 empty
    ///   6 tile  → 2 upgraded + 4 empty
    ///   7 tile  → 2 upgraded + 1 same + 4 empty
    ///   9 tile  → 3 upgraded + 6 empty
    /// </summary>
    public class MergeHelperTests
    {
        // ─── Sahne state ──────────────────────────────────────────────────────
        private readonly List<GameObject>                      _sceneObjects = new();
        private readonly Dictionary<TileNode, GameObject>     _nodeQuads    = new();
        private readonly Dictionary<TileNode, TileObjectValue> _mergeResults = new();

        private bool?      _lastCanMergeEvent;
        private GameObject _stepLabel;
        private EventService _eventService;
        private ITileObjectMergeHelper _mergeHelper;

        // ─── Sabitler ─────────────────────────────────────────────────────────
        private static readonly TileObjectValue TypeA1 = new TileObjectValue(1, 1);
        private static readonly TileObjectValue TypeA2 = new TileObjectValue(1, 2);
        private static readonly TileObjectValue TypeA3 = new TileObjectValue(1, 3);
        private static readonly TileObjectValue TypeB1 = new TileObjectValue(2, 1);
        private static readonly TileObjectValue Empty   = TileObjectValue.GetEmptyTileObjectValue();

        private const float STEP_DELAY   = 1.2f;
        private const float RESULT_DELAY = 1.8f;
        private const float TILE_SIZE    = 0.85f;
        private const float TILE_SPACING = 1.15f;

        // ─── Setup / TearDown ─────────────────────────────────────────────────
        [SetUp]
        public void SetUp()
        {
            _mergeResults.Clear();
            _nodeQuads.Clear();
            _lastCanMergeEvent = null;
            _eventService = new EventService();
            _mergeHelper = new TileObjectMergeHelper(_eventService);
            _eventService.OnCanMergeStateChange += OnCanMergeStateChange;
            _mergeHelper.MergeCancel();
        }

        [TearDown]
        public void TearDown()
        {
            _mergeHelper.MergeCancel();
            _eventService.OnCanMergeStateChange -= OnCanMergeStateChange;
            foreach (var go in _sceneObjects)
                if (go != null) Object.Destroy(go);
            _sceneObjects.Clear();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CanMerge — Erken çıkış
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator CanMerge_EmptyTargetValue_ReturnsFalse()
        {
            SetupCamera("CanMerge: Empty targetValue → false");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "current", TypeA1, LinearPos(0, 2));
            var moved   = MakeNode(graph, "moved",   TypeA1, LinearPos(1, 2));
            AddEdge(current, moved);
            DrawConnections(graph);
            SetLabel(current, "current");
            SetLabel(moved,   "moved");
            yield return new WaitForSeconds(STEP_DELAY);

            bool result = _mergeHelper.CanMerge(current, moved, Empty, false);
            MarkResult(current, result); MarkResult(moved, result);

            Assert.IsFalse(result);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_EmptyMovedNodeValue_ReturnsFalse()
        {
            SetupCamera("CanMerge: moved node empty → false");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "current", TypeA1, LinearPos(0, 2));
            var moved   = MakeNode(graph, "moved",   Empty,   LinearPos(1, 2));
            AddEdge(current, moved);
            DrawConnections(graph);
            SetLabel(current, "cur\nA1");
            SetLabel(moved,   "mov\nBOŞ");
            yield return new WaitForSeconds(STEP_DELAY);

            bool result = _mergeHelper.CanMerge(current, moved, TypeA1, false);
            MarkResult(current, result); MarkResult(moved, result);

            Assert.IsFalse(result);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CanMerge — Sayı kontrolü
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator CanMerge_TwoConnectedTiles_ReturnsFalse()
        {
            SetupCamera("CanMerge: 2 tile → merge yok (min 3 gerekli)");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 2));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 2));
            AddEdge(current, moved);
            DrawConnections(graph);
            SetLabel(current, "A\nsrc");
            SetLabel(moved,   "B\nhdf");
            yield return new WaitForSeconds(STEP_DELAY);

            bool result = _mergeHelper.CanMerge(current, moved, TypeA1, false);
            MarkResult(current, result); MarkResult(moved, result);

            Assert.IsFalse(result);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_ThreeConnectedTiles_ReturnsTrue()
        {
            SetupCamera("CanMerge: 3 tile → merge mümkün ✓");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A\nsrc");
            SetLabel(moved,   "B\nhdf");
            SetLabel(extra,   "C");
            yield return new WaitForSeconds(STEP_DELAY);

            bool result = _mergeHelper.CanMerge(current, moved, TypeA1, false, out var wantedNodes);
            HighlightWantedNodes(wantedNodes, result);

            Assert.IsTrue(result);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_DifferentTypeTileBreaksChain_ReturnsFalse()
        {
            SetupCamera("CanMerge: Farklı tip araya girince zincir kırılıyor");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 4));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 4));
            var blocker = MakeNode(graph, "X", TypeB1, LinearPos(2, 4));
            var other   = MakeNode(graph, "C", TypeA1, LinearPos(3, 4));
            AddEdge(current, moved); AddEdge(moved, blocker); AddEdge(blocker, other);
            DrawConnections(graph);
            SetLabel(current, "A"); SetLabel(moved, "B\nhdf");
            SetLabel(blocker, "X\nB2\nENGL"); SetLabel(other, "C");
            yield return new WaitForSeconds(STEP_DELAY);

            bool result = _mergeHelper.CanMerge(current, moved, TypeA1, false, out var wantedNodes);
            HighlightWantedNodes(wantedNodes, result);
            _nodeQuads[blocker].GetComponent<Renderer>().material.color = new Color(1f, 0.25f, 0.1f);

            Assert.IsFalse(result);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_WithIndicator_ScalesUpMergeableNodes()
        {
            SetupCamera("CanMerge: indicator=true → tile büyüyor");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A\nsrc"); SetLabel(moved, "B\nhdf"); SetLabel(extra, "C");
            yield return new WaitForSeconds(STEP_DELAY);

            // indicateMergeableObjects = true → onUpdateMergeableIndicator(true) çağrılır
            bool result = _mergeHelper.CanMerge(current, moved, TypeA1, indicateMergeableObjects: true);

            Assert.IsTrue(result);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CanMerge — wantedNodes çıktısı
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator CanMerge_WantedNodes_AlwaysIncludesCurrentNode()
        {
            SetupCamera("CanMerge: wantedNodes her zaman current içerir");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A\ncur"); SetLabel(moved, "B"); SetLabel(extra, "C");
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.CanMerge(current, moved, TypeA1, false, out var wantedNodes);
            HighlightWantedNodes(wantedNodes, true);

            Assert.IsTrue(wantedNodes.Contains(current));
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_WantedNodes_CountCorrect_ThreeTiles()
        {
            SetupCamera("CanMerge: wantedNodes.Count == 3");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A"); SetLabel(moved, "B"); SetLabel(extra, "C");
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.CanMerge(current, moved, TypeA1, false, out var wantedNodes);
            HighlightWantedNodes(wantedNodes, true);

            Assert.AreEqual(3, wantedNodes.Count);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_DisconnectedGroups_OnlyConnectedGroupCounts()
        {
            SetupCamera("CanMerge: Bağlantısız 2 grup — sadece bağlı grup sayılır");
            var graph = new TileGraph();
            // Grup 1: A - B (2 tile, bağlı)
            var current = MakeNode(graph, "A", TypeA1, new Vector3(-2.3f,  0.5f, 0));
            var moved   = MakeNode(graph, "B", TypeA1, new Vector3(-1.15f, 0.5f, 0));
            // Grup 2: C - D - E (3 tile, bağlı ama ayrı)
            var c = MakeNode(graph, "C", TypeA1, new Vector3( 0.6f, -0.5f, 0));
            var d = MakeNode(graph, "D", TypeA1, new Vector3( 1.75f,-0.5f, 0));
            var e = MakeNode(graph, "E", TypeA1, new Vector3( 2.9f, -0.5f, 0));
            AddEdge(current, moved);
            AddEdge(c, d); AddEdge(d, e);
            DrawConnections(graph);
            SetLabel(current, "A\nsrc"); SetLabel(moved, "B\nhdf");
            SetLabel(c, "C"); SetLabel(d, "D"); SetLabel(e, "E");
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.CanMerge(current, moved, TypeA1, false, out var wantedNodes);
            HighlightWantedNodes(wantedNodes, false);

            // A grubundan sadece 2 tile — merge yok
            Assert.IsFalse(wantedNodes.Count >= 3);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CanMerge — EventService
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator CanMerge_FiresCanMergeStateChangeEvent_WhenTrue()
        {
            SetupCamera("CanMerge: onCanMergeStateChange(true) ateşleniyor");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.CanMerge(current, moved, TypeA1, false);
            MarkResult(current, true); MarkResult(moved, true); MarkResult(extra, true);

            Assert.AreEqual(true, _lastCanMergeEvent);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator CanMerge_FiresCanMergeStateChangeEvent_WhenFalse()
        {
            SetupCamera("CanMerge: onCanMergeStateChange(false) ateşleniyor");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 2));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 2));
            AddEdge(current, moved);
            DrawConnections(graph);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.CanMerge(current, moved, TypeA1, false);
            MarkResult(current, false); MarkResult(moved, false);

            Assert.AreEqual(false, _lastCanMergeEvent);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — Başarısız merge
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_TwoTiles_ReturnsFalse_NoMergeOccurs()
        {
            SetupCamera("TryMerge: 2 tile — merge olmaz");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 2));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 2));
            AddEdge(current, moved);
            DrawConnections(graph);
            SetLabel(current, "A\nsrc"); SetLabel(moved, "B\nhdf");
            SubscribeMergeResult(current, moved);
            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(current, moved, TypeA1);
            MarkResult(current, merged); MarkResult(moved, merged);

            Assert.IsFalse(merged);
            Assert.IsEmpty(_mergeResults);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — 3 tile
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_ThreeTiles_ReturnsTrueAndInvokesMerged()
        {
            SetupCamera("TryMerge: 3 tile → merge gerçekleşiyor");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A\nsrc"); SetLabel(moved, "B\nhdf"); SetLabel(extra, "C");
            SubscribeMergeResult(current, moved, extra);
            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(current, moved, TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(merged);
            Assert.AreEqual(3, _mergeResults.Count);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator TryMerge_ThreeTiles_MovedNodeGetsUpgradedLevel()
        {
            SetupCamera("TryMerge: 3 tile — B (moved) Cyan'a dönüşür");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A"); SetLabel(moved, "B\nhdf→L2"); SetLabel(extra, "C");
            SubscribeMergeResult(current, moved, extra);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.TryMerge(current, moved, TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(_mergeResults[moved].Equals(TypeA2),
                $"moved node TypeA2 almalıydı, aldığı: {_mergeResults[moved]}");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator TryMerge_ThreeTiles_RemainingNodesGetEmpty()
        {
            SetupCamera("TryMerge: 3 tile — 2 node gri (empty) olur");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A\n→boş"); SetLabel(moved, "B\n→L2"); SetLabel(extra, "C\n→boş");
            SubscribeMergeResult(current, moved, extra);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.TryMerge(current, moved, TypeA1);
            UpdateQuadColors();

            Assert.AreEqual(2, CountResults(v => v.IsEmptyTileObjectValue()));
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — 4 tile
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_FourTiles_OneUpgradedOneSameLevel_TwoEmpty()
        {
            SetupCamera("TryMerge: 4 tile → 1 Lv2 + 1 Lv1 + 2 empty");
            var graph = new TileGraph();
            var ns    = LinearNodes(graph, 4, TypeA1);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SetLabel(ns[2], "C"); SetLabel(ns[3], "D");
            SubscribeMergeResult(ns);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            UpdateQuadColors();

            Assert.AreEqual(1, CountResults(v => v.Equals(TypeA2)), "1 upgraded");
            Assert.AreEqual(1, CountResults(v => v.Equals(TypeA1)), "1 same level");
            Assert.AreEqual(2, CountResults(v => v.IsEmptyTileObjectValue()), "2 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — 5 tile
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_FiveTiles_TwoUpgraded_ThreeEmpty()
        {
            SetupCamera("TryMerge: 5 tile → 2 Lv2 + 3 empty");
            var graph = new TileGraph();
            var ns    = LinearNodes(graph, 5, TypeA1);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SubscribeMergeResult(ns);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            UpdateQuadColors();

            Assert.AreEqual(2, CountResults(v => v.Equals(TypeA2)), "2 upgraded");
            Assert.AreEqual(3, CountResults(v => v.IsEmptyTileObjectValue()), "3 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — 6 tile (YENİ)
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_SixTiles_TwoUpgraded_FourEmpty()
        {
            SetupCamera("TryMerge: 6 tile → 2 Lv2 + 4 empty", orthoSize: 5f);
            var graph = new TileGraph();
            var ns    = LinearNodes(graph, 6, TypeA1);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SubscribeMergeResult(ns);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            UpdateQuadColors();

            Assert.AreEqual(2, CountResults(v => v.Equals(TypeA2)), "2 upgraded");
            Assert.AreEqual(4, CountResults(v => v.IsEmptyTileObjectValue()), "4 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — 9 tile (YENİ)
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_NineTiles_ThreeUpgraded_SixEmpty()
        {
            SetupCamera("TryMerge: 9 tile → 3 Lv2 + 6 empty", orthoSize: 6f);
            var graph = new TileGraph();
            var ns    = LinearNodes(graph, 9, TypeA1);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SubscribeMergeResult(ns);
            yield return new WaitForSeconds(STEP_DELAY);

            _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            UpdateQuadColors();

            Assert.AreEqual(3, CountResults(v => v.Equals(TypeA2)), "3 upgraded");
            Assert.AreEqual(6, CountResults(v => v.IsEmptyTileObjectValue()), "6 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — Zincir merge: Level 2 → Level 3 (YENİ)
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_LevelTwoTiles_UpgradesToLevelThree()
        {
            SetupCamera("TryMerge: 3× Lv2 (Cyan) → Lv3 (Yeşil)");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA2, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA2, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA2, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A\nL2"); SetLabel(moved, "B\nL2\nhdf"); SetLabel(extra, "C\nL2");
            SubscribeMergeResult(current, moved, extra);
            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(current, moved, TypeA2);
            UpdateQuadColors();

            Assert.IsTrue(merged);
            Assert.AreEqual(1, CountResults(v => v.Equals(TypeA3)), "1 Lv3 bekleniyor");
            Assert.AreEqual(2, CountResults(v => v.IsEmptyTileObjectValue()), "2 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — L-şekilli grid (YENİ)
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_LShapedLayout_MergeFindsAllConnected()
        {
            SetupCamera("TryMerge: L-şekilli grid — 5 tile merge");
            var graph = new TileGraph();
            //  A - B - C
            //          |
            //          D
            //          |
            //          E
            var a = MakeNode(graph, "A", TypeA1, new Vector3(-1.15f,  0.55f, 0));
            var b = MakeNode(graph, "B", TypeA1, new Vector3(    0f,  0.55f, 0));
            var c = MakeNode(graph, "C", TypeA1, new Vector3( 1.15f,  0.55f, 0));
            var d = MakeNode(graph, "D", TypeA1, new Vector3( 1.15f, -0.55f, 0));
            var e = MakeNode(graph, "E", TypeA1, new Vector3( 1.15f, -1.65f, 0));
            AddEdge(a, b); AddEdge(b, c); AddEdge(c, d); AddEdge(d, e);
            DrawConnections(graph);
            SetLabel(a, "A\nsrc"); SetLabel(b, "B\nhdf");
            SubscribeMergeResult(a, b, c, d, e);
            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(a, b, TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(merged);
            Assert.AreEqual(2, CountResults(v => v.Equals(TypeA2)), "5 tile → 2 upgraded");
            Assert.AreEqual(3, CountResults(v => v.IsEmptyTileObjectValue()), "3 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TryMerge — 2x3 Grid (YENİ)
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator TryMerge_2x3Grid_SixTiles_TwoUpgraded()
        {
            SetupCamera("TryMerge: 2×3 grid — 6 tile merge");
            var graph = new TileGraph();
            //  A - B - C
            //  |   |   |
            //  D - E - F
            var pos = new Vector3[]
            {
                new(-1.15f,  0.55f, 0), // A
                new(    0f,  0.55f, 0), // B
                new( 1.15f,  0.55f, 0), // C
                new(-1.15f, -0.55f, 0), // D
                new(    0f, -0.55f, 0), // E
                new( 1.15f, -0.55f, 0), // F
            };
            string[] names = { "A", "B", "C", "D", "E", "F" };
            var ns = new TileNode[6];
            for (int i = 0; i < 6; i++)
                ns[i] = MakeNode(graph, names[i], TypeA1, pos[i]);

            AddEdge(ns[0], ns[1]); AddEdge(ns[1], ns[2]);
            AddEdge(ns[3], ns[4]); AddEdge(ns[4], ns[5]);
            AddEdge(ns[0], ns[3]); AddEdge(ns[1], ns[4]); AddEdge(ns[2], ns[5]);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SubscribeMergeResult(ns);
            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(merged);
            Assert.AreEqual(2, CountResults(v => v.Equals(TypeA2)), "2 upgraded");
            Assert.AreEqual(4, CountResults(v => v.IsEmptyTileObjectValue()), "4 empty");
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // MergeCancel
        // ═══════════════════════════════════════════════════════════════════════

        [UnityTest]
        public IEnumerator MergeCancel_FiresCanMergeStateChangeFalse()
        {
            SetupCamera("MergeCancel → onCanMergeStateChange(false)");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 2));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 2));
            AddEdge(current, moved);
            DrawConnections(graph);
            yield return new WaitForSeconds(STEP_DELAY);

            _lastCanMergeEvent = null;
            _mergeHelper.MergeCancel();

            Assert.AreEqual(false, _lastCanMergeEvent);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        [UnityTest]
        public IEnumerator MergeCancel_AfterCanMerge_HidesIndicators()
        {
            SetupCamera("MergeCancel: indicator açık → kapatıldı");
            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, LinearPos(0, 3));
            var moved   = MakeNode(graph, "B", TypeA1, LinearPos(1, 3));
            var extra   = MakeNode(graph, "C", TypeA1, LinearPos(2, 3));
            AddEdge(current, moved); AddEdge(moved, extra);
            DrawConnections(graph);
            SetLabel(current, "A"); SetLabel(moved, "B"); SetLabel(extra, "C");
            // indicator açık göster
            _mergeHelper.CanMerge(current, moved, TypeA1, indicateMergeableObjects: true);
            yield return new WaitForSeconds(STEP_DELAY);

            // Cancel: indicator kapanır (tile'lar normal boyuta döner)
            _mergeHelper.MergeCancel();

            Assert.AreEqual(false, _lastCanMergeEvent);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // İki Adımlı Merge
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 9 tile Lv1 → Adım 1: 3 Lv2 üretilir (bağlı)
        ///              → Adım 2: 3 Lv2 merge → 1 Lv3
        /// </summary>
        [UnityTest]
        public IEnumerator TwoStepMerge_NineLv1Tiles_ProducesThreeLv2_ThenMergesIntoLv3()
        {
            SetupCamera("2 Adım: 9× Lv1 → 3× Lv2 → 1× Lv3", orthoSize: 6f);
            var graph = new TileGraph();
            var ns    = LinearNodes(graph, 9, TypeA1);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SubscribeMergeResult(ns);

            ShowStepText("Adım 1 — 9× Lv1 merge ediliyor");
            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Adım 1: 9 Lv1 → [B,C,D]=Lv2, geri kalanlar=Empty ───
            bool step1 = _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            ApplyMergeResultsToNodes();

            Assert.IsTrue(step1, "Adım 1 merge gerçekleşmeli");
            Assert.AreEqual(3, CountResults(v => v.Equals(TypeA2)), "3 Lv2 oluşmalı");
            Assert.AreEqual(6, CountResults(v => v.IsEmptyTileObjectValue()), "6 empty olmalı");

            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Adım 2: ns[1](B)=Lv2 src, ns[2](C)=Lv2 hdf → merge ───
            // BFS(C, except B): C→D(Lv2) + add B → [C,D,B] = 3 Lv2 node
            ShowStepText("Adım 2 — 3× Lv2 merge ediliyor");
            _mergeResults.Clear();
            _mergeHelper.MergeCancel();

            bool step2 = _mergeHelper.TryMerge(ns[1], ns[2], TypeA2);
            UpdateQuadColors();

            Assert.IsTrue(step2, "Adım 2 merge gerçekleşmeli");
            Assert.AreEqual(1, CountResults(v => v.Equals(TypeA3)), "1 Lv3 oluşmalı");
            Assert.AreEqual(2, CountResults(v => v.IsEmptyTileObjectValue()), "2 empty olmalı");

            yield return new WaitForSeconds(RESULT_DELAY);
        }

        /// <summary>
        /// 7 tile Lv1 → Adım 1: 2 Lv2 + 1 Lv1 üretilir
        ///              → Adım 2: sadece 2 Lv2 bağlı → merge YOK (min 3 gerekli)
        /// </summary>
        [UnityTest]
        public IEnumerator TwoStepMerge_SevenLv1Tiles_OnlyTwoLv2_CannotMergeInStep2()
        {
            SetupCamera("2 Adım: 7× Lv1 → 2× Lv2 → merge yok!", orthoSize: 5.5f);
            var graph = new TileGraph();
            var ns    = LinearNodes(graph, 7, TypeA1);
            DrawConnections(graph);
            SetLabel(ns[0], "A\nsrc"); SetLabel(ns[1], "B\nhdf");
            SubscribeMergeResult(ns);

            ShowStepText("Adım 1 — 7× Lv1 merge ediliyor");
            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Adım 1: 7 Lv1 → [B=Lv2, C=Lv2, D=Lv1(same), E..G=Empty, A=Empty] ───
            bool step1 = _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            ApplyMergeResultsToNodes();

            Assert.IsTrue(step1, "Adım 1 merge gerçekleşmeli");
            Assert.AreEqual(2, CountResults(v => v.Equals(TypeA2)), "2 Lv2 oluşmalı");
            Assert.AreEqual(1, CountResults(v => v.Equals(TypeA1)), "1 Lv1 (aynı seviye) kalmalı");

            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Adım 2: ns[1](Lv2)=src, ns[2](Lv2)=hdf → sadece 2 bağlı Lv2 ───
            // BFS(C, except B): C'nin komşusu D=Lv1 (yanlış değer) → sadece [C]+B = 2 node
            ShowStepText("Adım 2 — 2× Lv2 yetersiz, merge olmuyor");
            _mergeResults.Clear();
            _mergeHelper.MergeCancel();

            bool step2 = _mergeHelper.TryMerge(ns[1], ns[2], TypeA2);

            Assert.IsFalse(step2, "Adım 2: 2 Lv2 var, 3 gerekli → merge olmamalı");
            Assert.IsEmpty(_mergeResults, "onTileObjectMerged çağrılmamalı");

            yield return new WaitForSeconds(RESULT_DELAY);
        }

        /// <summary>
        /// 3 bağımsız grup, her biri 3× Lv1 → ayrı ayrı merge → her biri 1 Lv2 üretir.
        /// Adım 2: 3 Lv2 node birbirine bağlanır → merge → 1 Lv3
        /// </summary>
        [UnityTest]
        public IEnumerator TwoStepMerge_ThreeSeparateGroups_Lv2ConnectedAfterMerge_ProducesLv3()
        {
            // Dikey gruplar: her grup sTop-sMid-b-t olarak dizilir.
            // b (hedef) nodeları y=0, x=-1.15 / 0 / +1.15 → doğal olarak TILE_SPACING arayla.
            // Adım 1 sonrası b nodeları Lv2 olur; zaten yan yana oldukları için AddEdge mantıklı görünür.
            // T (boş) nodeları b'nin altında: "nodeun üzerinde item olmak zorunda değil" gösterimi.
            SetupCamera("2 Adım: 3 Dikey Grup → Lv2 Komşu → Lv3", orthoSize: 5f);
            var graph = new TileGraph();

            // Grup merkezi = b node'unun konumu (y=0 satırı)
            var g1 = CreateGroup(graph, "1", new Vector3(-TILE_SPACING, 0, 0));
            var g2 = CreateGroup(graph, "2", new Vector3(           0f, 0, 0));
            var g3 = CreateGroup(graph, "3", new Vector3(+TILE_SPACING, 0, 0));
            // g[0]=sTop(src)  g[1]=sMid  g[2]=b(hedef→Lv2)  g[3]=t(boş)

            DrawConnections(graph);
            SetLabel(g1[0], "St1\nsrc"); SetLabel(g1[2], "B1\nhdf"); SetLabel(g1[3], "T1\nboş");
            SetLabel(g2[0], "St2\nsrc"); SetLabel(g2[2], "B2\nhdf"); SetLabel(g2[3], "T2\nboş");
            SetLabel(g3[0], "St3\nsrc"); SetLabel(g3[2], "B3\nhdf"); SetLabel(g3[3], "T3\nboş");
            SubscribeMergeResult(g1); SubscribeMergeResult(g2); SubscribeMergeResult(g3);

            ShowStepText("Adım 1 — Her grup kendi içinde merge oluyor");
            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Adım 1: Her grup: sTop(src) → b(hedef), BFS b'den except sTop → b+sMid+sTop=3 ───
            _mergeHelper.TryMerge(g1[0], g1[2], TypeA1);
            ApplyMergeResultsToNodes(); _mergeResults.Clear(); _mergeHelper.MergeCancel();

            _mergeHelper.TryMerge(g2[0], g2[2], TypeA1);
            ApplyMergeResultsToNodes(); _mergeResults.Clear(); _mergeHelper.MergeCancel();

            _mergeHelper.TryMerge(g3[0], g3[2], TypeA1);
            ApplyMergeResultsToNodes();

            // b nodeları (g_[2]) Lv2 olmalı
            Assert.IsTrue(g1[2].Value.Equals(TypeA2), "g1[2] Lv2 olmalı");
            Assert.IsTrue(g2[2].Value.Equals(TypeA2), "g2[2] Lv2 olmalı");
            Assert.IsTrue(g3[2].Value.Equals(TypeA2), "g3[2] Lv2 olmalı");

            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Lv2 b nodelarını birbirine bağla ───
            // g1[2](-1.15,0) ↔ g2[2](0,0) ↔ g3[2](+1.15,0): mesafe = TILE_SPACING → doğal komşu
            AddEdge(g1[2], g2[2]);
            AddEdge(g2[2], g3[2]);
            DrawLine(g1[2].Transform.position, g2[2].Transform.position, new Color(1f, 0.85f, 0.1f));
            DrawLine(g2[2].Transform.position, g3[2].Transform.position, new Color(1f, 0.85f, 0.1f));

            ShowStepText("Adım 2 — Lv2 b nodeları komşu → merge → Lv3!");
            yield return new WaitForSeconds(STEP_DELAY);

            // ─── Adım 2: g1[2](src)=Lv2, g2[2](hdf)=Lv2 → BFS g2[2] except g1[2] → g3[2] bulunur ───
            _mergeResults.Clear();
            SubscribeMergeResult(g1[2], g2[2], g3[2]);
            _mergeHelper.MergeCancel();

            bool step2 = _mergeHelper.TryMerge(g1[2], g2[2], TypeA2);
            UpdateQuadColors();

            Assert.IsTrue(step2, "Adım 2 merge gerçekleşmeli");
            Assert.AreEqual(1, CountResults(v => v.Equals(TypeA3)), "1 Lv3 oluşmalı");
            Assert.AreEqual(2, CountResults(v => v.IsEmptyTileObjectValue()), "2 node boşalmalı");

            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Sahne kurulum helpers
        // ═══════════════════════════════════════════════════════════════════════

        private void SetupCamera(string label, float orthoSize = 4f)
        {
            var camGo = new GameObject("TestCamera");
            _sceneObjects.Add(camGo);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = orthoSize;
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0, 0, -10f);

            var titleGo = new GameObject("Title");
            _sceneObjects.Add(titleGo);
            titleGo.transform.position = new Vector3(0, orthoSize - 0.5f, 0);
            titleGo.transform.localScale = Vector3.one * 0.22f;
            var tm = titleGo.AddComponent<TextMesh>();
            tm.text = label;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.UpperCenter;
            tm.fontSize = 24;
            tm.color = new Color(0.9f, 0.9f, 0.6f);
        }

        private TileNode MakeNode(TileGraph graph, string name, TileObjectValue value, Vector3 worldPos)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = $"Tile_{name}";
            quad.transform.position = worldPos;
            quad.transform.localScale = Vector3.one * TILE_SIZE;
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = ColorForValue(value);
            quad.GetComponent<Renderer>().material = mat;
            _sceneObjects.Add(quad);

            var nodeGo = new GameObject($"Node_{name}");
            nodeGo.transform.position = worldPos;
            _sceneObjects.Add(nodeGo);

            var node = new TileNode(nodeGo.transform, value);

            // Indicator → tile scale ile göster
            node.onUpdateMergeableIndicator += indicated =>
            {
                if (_nodeQuads.TryGetValue(node, out var q))
                    q.transform.localScale = indicated
                        ? Vector3.one * (TILE_SIZE * 1.2f)
                        : Vector3.one * TILE_SIZE;
            };

            _nodeQuads[node] = quad;
            graph.AddNode(node);
            return node;
        }

        private TileNode[] LinearNodes(TileGraph graph, int count, TileObjectValue value)
        {
            var ns = new TileNode[count];
            for (int i = 0; i < count; i++)
                ns[i] = MakeNode(graph, ((char)('A' + i)).ToString(), value, LinearPos(i, count));
            for (int i = 0; i < count - 1; i++)
                AddEdge(ns[i], ns[i + 1]);
            return ns;
        }

        private void DrawConnections(TileGraph graph)
        {
            foreach (var node in graph.GetNodes())
            {
                foreach (var neighbor in node.GetNeighbors())
                {
                    if (node.GetHashCode() > neighbor.GetHashCode()) continue;
                    var lineGo = new GameObject("Edge");
                    _sceneObjects.Add(lineGo);
                    var lr = lineGo.AddComponent<LineRenderer>();
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.material.color = new Color(0.55f, 0.55f, 0.55f, 0.45f);
                    lr.startWidth = lr.endWidth = 0.05f;
                    lr.positionCount = 2;
                    lr.SetPosition(0, node.Transform.position     + Vector3.forward * 0.1f);
                    lr.SetPosition(1, neighbor.Transform.position + Vector3.forward * 0.1f);
                    lr.useWorldSpace = true;
                }
            }
        }

        private void SetLabel(TileNode node, string text)
        {
            // Her satırı tile yüzeyine (z=-0.15) ayrı TextMesh olarak bas.
            // Bu şekilde komşu tile'larla veya alt satır tile'larla çakışma olmaz.
            var tilePos = _nodeQuads[node].transform.position;
            var lines   = text.Split('\n');
            float lineH = 0.20f;
            float totalH = (lines.Length - 1) * lineH;

            for (int i = 0; i < lines.Length; i++)
            {
                var go = new GameObject($"Lbl_{node.Name}_{i}");
                _sceneObjects.Add(go);
                float yOff = totalH * 0.5f - i * lineH;
                go.transform.position   = tilePos + new Vector3(0, yOff, -0.15f);
                go.transform.localScale = Vector3.one * (i == 0 ? 0.17f : 0.12f);
                var tm = go.AddComponent<TextMesh>();
                tm.text      = lines[i];
                tm.alignment = TextAlignment.Center;
                tm.anchor    = TextAnchor.MiddleCenter;
                tm.fontSize  = i == 0 ? 22 : 18;
                tm.color     = Color.white;
            }
        }

        private void HighlightWantedNodes(List<TileNode> wantedNodes, bool canMerge)
        {
            if (wantedNodes == null) return;
            var highlightColor = canMerge ? new Color(1f, 0.85f, 0f) : new Color(1f, 0.3f, 0.3f);
            foreach (var n in wantedNodes)
            {
                if (_nodeQuads.TryGetValue(n, out var q))
                    q.GetComponent<Renderer>().material.color = highlightColor;
            }
        }

        private void MarkResult(TileNode node, bool success)
        {
            if (!_nodeQuads.TryGetValue(node, out var q)) return;
            q.GetComponent<Renderer>().material.color = success
                ? new Color(0.2f, 0.9f, 0.3f)
                : new Color(0.9f, 0.2f, 0.2f);
        }

        private void UpdateQuadColors()
        {
            foreach (var (node, quad) in _nodeQuads)
            {
                if (!_mergeResults.TryGetValue(node, out var newValue)) continue;
                quad.GetComponent<Renderer>().material.color = ColorForValue(newValue);
                quad.transform.localScale = newValue.IsEmptyTileObjectValue()
                    ? Vector3.one * (TILE_SIZE * 0.45f)
                    : Vector3.one * TILE_SIZE;
            }
        }

        // ─── İki adımlı merge helpers ─────────────────────────────────────────

        /// <summary>
        /// _mergeResults'taki değerleri ilgili node'lara uygular ve görseli günceller.
        /// 2 adımlı merge testlerinde adımlar arası node.Value güncel tutmak için zorunludur.
        /// </summary>
        private void ApplyMergeResultsToNodes()
        {
            foreach (var (node, value) in _mergeResults)
                node.SetValue(value);
            UpdateQuadColors();
        }

        /// <summary>
        /// Ekranın altında adım göstergesi basar; önceki varsa yerini alır.
        /// </summary>
        private void ShowStepText(string text)
        {
            if (_stepLabel != null) Object.Destroy(_stepLabel);
            _stepLabel = new GameObject("StepLabel");
            _sceneObjects.Add(_stepLabel);
            _stepLabel.transform.position   = new Vector3(0, -3.5f, 0);
            _stepLabel.transform.localScale = Vector3.one * 0.19f;
            var tm = _stepLabel.AddComponent<TextMesh>();
            tm.text      = text;
            tm.alignment = TextAlignment.Center;
            tm.anchor    = TextAnchor.MiddleCenter;
            tm.fontSize  = 20;
            tm.color     = new Color(0.75f, 0.95f, 1f);
        }

        /// <summary>
        /// 4 node'luk dikey grup oluşturur:
        ///   [0] sTop  — TypeA1, bPos + (0, 2×TILE_SPACING, 0)   → src (sürüklenen)
        ///   [1] sMid  — TypeA1, bPos + (0,   TILE_SPACING, 0)
        ///   [2] b     — TypeA1, bPos                             → hedef (Lv2 çıktısı)
        ///   [3] t     — Empty,  bPos + (0, -TILE_SPACING, 0)    → boş node (nesne yok)
        /// Merge: TryMerge(g[0], g[2], TypeA1) → BFS hedeften except src → b+sMid+sTop=3 → g[2] Lv2
        /// </summary>
        private TileNode[] CreateGroup(TileGraph graph, string suffix, Vector3 bPos)
        {
            var sTop = MakeNode(graph, $"St{suffix}", TypeA1, bPos + new Vector3(0, 2 * TILE_SPACING, 0));
            var sMid = MakeNode(graph, $"Sm{suffix}", TypeA1, bPos + new Vector3(0,     TILE_SPACING, 0));
            var b    = MakeNode(graph, $"B{suffix}",  TypeA1, bPos);
            var t    = MakeNode(graph, $"T{suffix}",  Empty,  bPos + new Vector3(0, -TILE_SPACING, 0));
            AddEdge(sTop, sMid);
            AddEdge(sMid, b);
            AddEdge(b, t);
            return new[] { sTop, sMid, b, t };
        }

        /// <summary>
        /// Verilen dünya koordinatları arasına renkli bir LineRenderer çizer.
        /// Yeni kenar bağlantılarını görsel olarak belirtmek için kullanılır.
        /// </summary>
        private void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            var go = new GameObject("DynamicEdge");
            _sceneObjects.Add(go);
            var lr = go.AddComponent<LineRenderer>();
            lr.material           = new Material(Shader.Find("Sprites/Default"));
            lr.material.color     = color;
            lr.startWidth         = lr.endWidth = 0.08f;
            lr.positionCount      = 2;
            lr.useWorldSpace      = true;
            lr.SetPosition(0, from + Vector3.forward * 0.08f);
            lr.SetPosition(1, to   + Vector3.forward * 0.08f);
        }

        // ──────────────────────────────────────────────────────────────────────

        private static void AddEdge(TileNode a, TileNode b)
        {
            a.AddNeighbor(b);
            b.AddNeighbor(a);
        }

        private void SubscribeMergeResult(params TileNode[] nodes)
        {
            foreach (var node in nodes)
            {
                var captured = node;
                captured.onTileObjectMerged += v => _mergeResults[captured] = v;
            }
        }

        private int CountResults(System.Func<TileObjectValue, bool> predicate)
        {
            int n = 0;
            foreach (var v in _mergeResults.Values)
                if (predicate(v)) n++;
            return n;
        }

        private static Vector3 LinearPos(int index, int total)
            => new Vector3((index - (total - 1) / 2f) * TILE_SPACING, 0, 0);

        private static Color ColorForValue(TileObjectValue v)
        {
            if (v.IsEmptyTileObjectValue()) return new Color(0.28f, 0.28f, 0.28f, 0.35f);
            return v.objectId switch
            {
                1 => v.objectLevel switch
                {
                    1 => new Color(0.2f, 0.45f, 1.0f),
                    2 => new Color(0.0f, 0.85f, 0.9f),
                    3 => new Color(0.1f, 0.9f,  0.3f),
                    _ => Color.white
                },
                2 => new Color(1.0f, 0.4f, 0.1f),
                _ => Color.white
            };
        }

        private void OnCanMergeStateChange(bool canMerge) => _lastCanMergeEvent = canMerge;
    }
}