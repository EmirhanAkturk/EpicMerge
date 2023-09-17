using Attribute;
using Systems.GraphSystem.Graphs;
using UnityEngine;
using Utils.Extensions;
using Random = UnityEngine.Random;

namespace Systems.GraphSystem.Test
{
   public class GraphTest : MonoBehaviour
   {
      [SerializeField] private GameObject nodePrefab;
      // [SerializeField] private float nodeDistance = 1.5f;
      [SerializeField] private Vector2Int matrixDimensions = new Vector2Int(5, 5);
      [SerializeField] private bool createGraphInStart = true;
      
      [Button(nameof(RecreateGraph))] public bool buttonField;

      private IntGraph graph;
   
      private void Start()
      {
         if (createGraphInStart)
         {
            CreateGraph();
         }
      }

      private void CreateGraph()
      {
         graph = new IntGraph();

         // Create Nodes
         int m = matrixDimensions.x;
         int n = matrixDimensions.y;

         CreateNodes(m, n);
         FindEdges();
      }

      private void CreateNodes(int m, int n)
      {
         int nodeCount = 0;
         for (int i = 0; i < m; i++)
         {
            for (int j = 0; j < n; j++)
            {
               int rndValue = Random.Range(0, 3);
               // Vector3 pos = new Vector3(i * nodeDistance, 0, j * nodeDistance);
               IntNode node = CreateNode(rndValue);
               ++nodeCount;

               graph.AddNode(node);
            }
         }
      }

      private void FindEdges()
      {
         // graph.FindEdgesWithNodeDistance(nodeDistance, true);
      }

      private IntNode CreateNode(int value)
      {
         // var nodeObject = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
         IntNode intNode = new IntNode(value);
         return intNode;
      }

      [ContextMenu("PrintGraphNeighbors")]
      public void PrintGraphNeighbors()
      {
         graph?.PrintGraphNeighbors();
      }      
      
      // [ContextMenu("RecreateGraph")]
      public void RecreateGraph()
      {
         DestroyImmediateGraph();
         CreateGraph();
      }

      private void DestroyImmediateGraph()
      {
         // graph?.DestroyImmediateNodes();
         graph = null;
      }
   }
}
