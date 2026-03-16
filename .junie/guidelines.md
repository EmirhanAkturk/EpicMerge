# Epic Merge Development Guidelines

This document provides essential information for developers working on the Epic Merge project.

## Build & Configuration

### Prerequisites
- **Unity 6000.0.62f1** or compatible.
- **.NET 4.7.1** runtime.

### Project Setup
1. Open the project in Unity.
2. Ensure all Assembly Definitions (`.asmdef`) are correctly compiled.
3. The project uses a custom `UpdateManager` to minimize `Update()` calls. Ensure this package is correctly linked if any issues occur with frame updates.

### Core Systems
- **Graph System**: Uses `Graph<T, TF>` and `Node<T, TF>` for tile representation.
- **Merge System**: Managed by `MergeHelper`. It uses BFS to find matching tiles.
- **Event System**: Follows the Observer pattern. See `EventService` for global events.
- **Indicator System**: Provides visual feedback for mergeable states.

## Testing

### Running Tests
The project uses the Unity Test Framework (NUnit).
- **EditMode Tests**: Located in `Assets/Tests/EditMode`. These test core logic (Graph, Nodes) without Unity Scene dependency.
- **PlayMode Tests**: Located in `Assets/Tests/PlayMode`. These test Unity-integrated behavior (Merge, Indicators, Detection).

To run tests:
1. Open **Window > General > Test Runner**.
2. Select either **EditMode** or **PlayMode** tab.
3. Click **Run All** or select specific tests.

### Adding New Tests
- **EditMode**: Use for pure C# logic. Inherit from standard NUnit tests.
- **PlayMode**: Use `[UnityTest]` and `IEnumerator` if you need to wait for frames or physics.
- **Conventions**:
    - Place tests in `Assets/Tests/[EditMode|PlayMode]`.
    - Use descriptive names (e.g., `Method_Condition_ExpectedResult`).
    - Mock Unity dependencies where possible for EditMode tests.

### Test Example
Below is a simple EditMode test demonstrating how to test the Graph BFS logic:

```csharp
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
            Assert.AreEqual(2, matchingNodes.Count);
            Assert.Contains(node1, matchingNodes);
            Assert.Contains(node2, matchingNodes);
            Assert.IsFalse(matchingNodes.Contains(node3));
        }
    }
}
```

## Additional Development Info

### Code Style
- **Naming**: Use PascalCase for classes and methods, camelCase for local variables.
- **Optimization**:
    - Avoid `Update()` in `MonoBehaviour`. Use `UpdateManager` instead.
    - Use `SpriteRenderer` instead of Mesh for 2D objects to reduce vertex count.
    - Leverage GPU Instancing and Sprite Atlases to minimize draw calls.
- **Architecture**:
    - The `TileGraph` is automatically populated based on node distances. See `TileGraphExtensions.FindEdgedWithNodeDistance`.
    - Detection logic is decoupled using interfaces (`IObjectDetector`, `IObjectDetectionHandler`).
