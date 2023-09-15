using Attribute;
using Systems.GraphSystem;
using UnityEngine;
using Utils.Extensions;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Systems.TileNodeSystem.Test
{
   public class TileNodeSystemTest : MonoBehaviour
   {
      [SerializeField] private GameObject nodePrefab;
      [SerializeField] private float nodeDistance = 1.5f;
      [SerializeField] private Vector2Int matrixDimensions = new Vector2Int(5, 5);
      [SerializeField] private bool createGraphInStart = true;
      
      [Button(nameof(RecreateGraph))] public bool buttonField;

      private Graph graph;
   
      private void Start()
      {
         if (createGraphInStart)
         {
            CreateGraph();
         }
      }

      private void CreateGraph()
      {
         graph = new Graph();

         // Create Nodes
         int m = matrixDimensions.x;
         int n = matrixDimensions.y;

         CreateNodes(m, n);
         FindEdges();
      }

      private void CreateNodes(int m, int n)
      {
         int nodeCount = 0;
         Vector3 parentPos = transform.position;
         for (int i = 0; i < m; i++)
         {
            for (int j = 0; j < n; j++)
            {
               int rndValue = Random.Range(0, 3);

               Vector3 localPos = new Vector3(i * nodeDistance, 0, j * nodeDistance);
               Vector3 pos = parentPos + localPos;
               
               TileNodeController tileNodeController = CreateNode(rndValue, pos);
               ++nodeCount;
               tileNodeController.gameObject.name = "Node_" + nodeCount;

               graph.AddNode(tileNodeController.TileNode);
            }
         }
      }

      private void FindEdges()
      {
         graph.FindEdgesWithNodeDistance(nodeDistance);
      }

      private TileNodeController CreateNode(int value, Vector3 pos)
      {
         var nodeObject = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
         var tileNodeController = nodeObject.GetComponent<TileNodeController>();
         tileNodeController.Init(value);
         return tileNodeController;
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
         graph?.DestroyImmediateNodes();
         graph = null;
      }
   }
}
