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
    /// Merge anını ekranda izlemek için görsel PlayMode testi.
    /// Her tile renkli bir kare olarak render edilir.
    /// Merge sonucunda renkler anlık olarak güncellenir.
    ///
    /// Renk şeması:
    ///   Level 1  → Mavi
    ///   Level 2  → Cyan (upgrade)
    ///   Level 3  → Yeşil (çift upgrade)
    ///   Empty    → Gri (yarı saydam)
    ///   Sürüklenen (current) → Sarı kenarlı
    /// </summary>
    public class MergeVisualTests
    {
        private readonly List<GameObject> _sceneObjects = new();

        // Her tile node → ekrandaki quad
        private readonly Dictionary<TileNode, GameObject> _nodeQuads = new();

        // Her tile node → merge sonucu
        private readonly Dictionary<TileNode, TileObjectValue> _mergeResults = new();

        private static readonly TileObjectValue TypeA1 = new TileObjectValue(1, 1);
        private static readonly TileObjectValue TypeA2 = new TileObjectValue(1, 2);
        private static readonly TileObjectValue TypeA3 = new TileObjectValue(1, 3);
        private static readonly TileObjectValue TypeB1 = new TileObjectValue(2, 1);
        private static readonly TileObjectValue Empty   = TileObjectValue.GetEmptyTileObjectValue();

        private const float STEP_DELAY     = 1.2f; // merge öncesi/sonrası bekleme
        private const float RESULT_DELAY   = 1.8f; // sonucu izleme süresi
        private const float TILE_SIZE      = 0.85f;
        private const float TILE_SPACING   = 1.1f;

        private ITileObjectMergeHelper _mergeHelper;

        [SetUp]
        public void SetUp()
        {
            _mergeResults.Clear();
            _nodeQuads.Clear();
            _mergeHelper = new TileObjectMergeHelper(new EventService());
            _mergeHelper.MergeCancel();
        }

        [TearDown]
        public void TearDown()
        {
            _mergeHelper.MergeCancel();
            foreach (var go in _sceneObjects)
                if (go != null) Object.Destroy(go);
            _sceneObjects.Clear();
        }

        // ─── Test 1: 3 tile merge ──────────────────────────────────────────

        [UnityTest]
        public IEnumerator Visual_ThreeTiles_MergeIntoOneUpgraded()
        {
            SetupCamera(label: "3 Tile Merge — Level 1 → Level 2");

            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, new Vector3(-1.1f, 0, 0));
            var moved   = MakeNode(graph, "B", TypeA1, new Vector3(   0f, 0, 0));
            var extra   = MakeNode(graph, "C", TypeA1, new Vector3( 1.1f, 0, 0));
            graph.AddEdge(current, moved);
            graph.AddEdge(moved,   extra);

            DrawConnections(graph);
            SubscribeMergeResult(current, moved, extra);
            SetLabel(current, "A\n(sürüklenen)");
            SetLabel(moved,   "B\n(hedef)");
            SetLabel(extra,   "C");

            yield return new WaitForSeconds(STEP_DELAY);

            // Merge!
            bool merged = _mergeHelper.TryMerge(current, moved, TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(merged, "Merge gerçekleşmeliydi");

            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ─── Test 2: 5 tile merge ──────────────────────────────────────────

        [UnityTest]
        public IEnumerator Visual_FiveTiles_MergeIntoTwoUpgraded()
        {
            SetupCamera(label: "5 Tile Merge — 2× Level 2 + 3 Empty");

            var graph = new TileGraph();
            var nodes = new TileNode[5];
            for (int i = 0; i < 5; i++)
            {
                float x = (i - 2) * TILE_SPACING;
                nodes[i] = MakeNode(graph, ((char)('A' + i)).ToString(), TypeA1, new Vector3(x, 0, 0));
            }
            for (int i = 0; i < 4; i++)
                graph.AddEdge(nodes[i], nodes[i + 1]);

            DrawConnections(graph);
            SubscribeMergeResult(nodes);
            SetLabel(nodes[0], "A\n(sürüklenen)");
            SetLabel(nodes[1], "B\n(hedef)");

            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(nodes[0], nodes[1], TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(merged);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ─── Test 3: Merge olamıyor — farklı tip araya giriyor ───────────────

        [UnityTest]
        public IEnumerator Visual_BlockedByDifferentType_NoMerge()
        {
            SetupCamera(label: "Merge Engellendi — Farklı Tip Araya Girdi");

            var graph   = new TileGraph();
            var current = MakeNode(graph, "A", TypeA1, new Vector3(-2.2f, 0, 0));
            var moved   = MakeNode(graph, "B", TypeA1, new Vector3(-1.1f, 0, 0));
            var blocker = MakeNode(graph, "X", TypeB1, new Vector3(   0f, 0, 0));
            var other   = MakeNode(graph, "C", TypeA1, new Vector3( 1.1f, 0, 0));
            var other2  = MakeNode(graph, "D", TypeA1, new Vector3( 2.2f, 0, 0));
            graph.AddEdge(current, moved);
            graph.AddEdge(moved,   blocker);
            graph.AddEdge(blocker, other);
            graph.AddEdge(other,   other2);

            DrawConnections(graph);
            SubscribeMergeResult(current, moved, blocker, other, other2);
            SetLabel(blocker, "X\n(engel/TypeB)");

            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(current, moved, TypeA1);
            // merge olmadı, renkler değişmez
            UpdateQuadColors();

            // Engeli kırmızı yap
            _nodeQuads[blocker].GetComponent<Renderer>().material.color = new Color(1f, 0.2f, 0.2f);

            Assert.IsFalse(merged);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ─── Test 4: Çift merge — 6 tile → 2 upgrade ─────────────────────────

        [UnityTest]
        public IEnumerator Visual_SixTiles_TwoUpgradesProduced()
        {
            SetupCamera(label: "6 Tile Merge — 2× Level 2 + 4 Empty");

            var graph = new TileGraph();
            // 2 satır, 3 sütun grid
            //  A - B - C
            //  |   |   |
            //  D - E - F
            var positions = new Vector3[]
            {
                new(-1.1f,  0.55f, 0), // A
                new(   0f,  0.55f, 0), // B
                new( 1.1f,  0.55f, 0), // C
                new(-1.1f, -0.55f, 0), // D
                new(   0f, -0.55f, 0), // E
                new( 1.1f, -0.55f, 0), // F
            };

            var ns = new TileNode[6];
            string[] names = { "A", "B", "C", "D", "E", "F" };
            for (int i = 0; i < 6; i++)
                ns[i] = MakeNode(graph, names[i], TypeA1, positions[i]);

            // yatay kenarlar
            graph.AddEdge(ns[0], ns[1]); graph.AddEdge(ns[1], ns[2]);
            graph.AddEdge(ns[3], ns[4]); graph.AddEdge(ns[4], ns[5]);
            // dikey kenarlar
            graph.AddEdge(ns[0], ns[3]); graph.AddEdge(ns[1], ns[4]); graph.AddEdge(ns[2], ns[5]);

            DrawConnections(graph);
            SubscribeMergeResult(ns);
            SetLabel(ns[0], "A\n(sürüklenen)");
            SetLabel(ns[1], "B\n(hedef)");

            yield return new WaitForSeconds(STEP_DELAY);

            bool merged = _mergeHelper.TryMerge(ns[0], ns[1], TypeA1);
            UpdateQuadColors();

            Assert.IsTrue(merged);
            yield return new WaitForSeconds(RESULT_DELAY);
        }

        // ─── Sahne Kurulum Helpers ─────────────────────────────────────────

        private void SetupCamera(string label)
        {
            var camGo = new GameObject("TestCamera");
            _sceneObjects.Add(camGo);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 4f;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0, 0, -10f);

            // Başlık etiketi
            var labelGo = new GameObject("Label_Title");
            _sceneObjects.Add(labelGo);
            labelGo.transform.position = new Vector3(0, 2.5f, 0);
        }

        private TileNode MakeNode(TileGraph graph, string name, TileObjectValue value, Vector3 worldPos)
        {
            // Quad oluştur
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"Tile_{name}";
            go.transform.position = worldPos;
            go.transform.localScale = Vector3.one * TILE_SIZE;
            _sceneObjects.Add(go);

            // Materyal rengi
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = ColorForValue(value);
            go.GetComponent<Renderer>().material = mat;

            // onUpdateMergeableIndicator null bırakılırsa NRE fırlatır
            var nodeGo = new GameObject($"Node_{name}");
            _sceneObjects.Add(nodeGo);
            nodeGo.transform.position = worldPos;

            var node = new TileNode(nodeGo.transform, value);
            node.onUpdateMergeableIndicator += _ => { };

            _nodeQuads[node] = go;
            graph.AddNode(node);
            return node;
        }

        private void DrawConnections(TileGraph graph)
        {
            foreach (var node in graph.GetNodes())
            {
                foreach (var neighbor in node.GetNeighbors())
                {
                    // LineRenderer ile edge çiz (tekrar çizimi önle)
                    if (System.Collections.Generic.Comparer<int>.Default.Compare(
                            node.GetHashCode(), neighbor.GetHashCode()) > 0) continue;

                    var lineGo = new GameObject("Edge");
                    _sceneObjects.Add(lineGo);
                    var lr = lineGo.AddComponent<LineRenderer>();
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.material.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                    lr.startWidth = lr.endWidth = 0.04f;
                    lr.positionCount = 2;
                    lr.SetPosition(0, node.Transform.position      + Vector3.forward * 0.1f);
                    lr.SetPosition(1, neighbor.Transform.position  + Vector3.forward * 0.1f);
                    lr.useWorldSpace = true;
                }
            }
        }

        private void SetLabel(TileNode node, string text)
        {
            // TextMesh ile basit etiket
            var go = new GameObject($"Label_{node.Name}");
            _sceneObjects.Add(go);
            go.transform.position = _nodeQuads[node].transform.position + new Vector3(0, -0.68f, -0.1f);
            go.transform.localScale = Vector3.one * 0.18f;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.UpperCenter;
            tm.fontSize = 28;
            tm.color = Color.white;
        }

        private void SubscribeMergeResult(params TileNode[] nodes)
        {
            foreach (var node in nodes)
            {
                var captured = node;
                captured.onTileObjectMerged += value =>
                {
                    _mergeResults[captured] = value;
                };
            }
        }

        private void UpdateQuadColors()
        {
            foreach (var (node, quad) in _nodeQuads)
            {
                if (_mergeResults.TryGetValue(node, out var newValue))
                {
                    quad.GetComponent<Renderer>().material.color = ColorForValue(newValue);
                    quad.transform.localScale = newValue.IsEmptyTileObjectValue()
                        ? Vector3.one * (TILE_SIZE * 0.5f)  // empty → küçül
                        : Vector3.one * TILE_SIZE;
                }
            }
        }

        // ─── Renk Yardımcıları ────────────────────────────────────────────

        private static Color ColorForValue(TileObjectValue v)
        {
            if (v.IsEmptyTileObjectValue())
                return new Color(0.3f, 0.3f, 0.3f, 0.3f);

            return v.objectId switch
            {
                1 => v.objectLevel switch
                {
                    1 => new Color(0.2f, 0.45f, 1.0f),   // mavi — Level 1
                    2 => new Color(0.0f, 0.85f, 0.9f),   // cyan — Level 2
                    3 => new Color(0.1f, 0.9f,  0.3f),   // yeşil — Level 3
                    _ => Color.white
                },
                2 => new Color(1.0f, 0.4f, 0.1f),        // turuncu — TypeB
                _ => Color.white
            };
        }
    }
}