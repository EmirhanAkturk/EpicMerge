using System;
using System.Collections.Generic;
using Attribute;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Systems.GraphSystem
{
   public class GraphTest : MonoBehaviour
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
         for (int i = 0; i < m; i++)
         {
            for (int j = 0; j < n; j++)
            {
               int rndValue = Random.Range(0, 3);

               TileNode node = CreateNode(rndValue, new Vector3(i * nodeDistance, 0, j * nodeDistance));
               ++nodeCount;
               node.gameObject.name = "Node_" + nodeCount;

               graph.AddNode(node);
            }
         }
      }

      private void FindEdges()
      {
         var nodes = graph.GetNodes();
         foreach (var node1 in nodes)
         {
            foreach (var node2 in nodes)
            {
               if(node1 == node2) continue;
               if (Vector3.Distance(node1.transform.position, node2.transform.position) <= nodeDistance)
               {
                  graph.AddEdge(node1, node2);
                  node1.CreateVertexObject(node2);
               }
            }
         }

         foreach (var node in nodes)
         {
            node.PrintNeighbors();
         }
      }

      private TileNode CreateNode(int value, Vector3 pos)
      {
         var nodeObject = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
         var node = nodeObject.GetComponent<TileNode>();
         node.Init(value);
         return node;
      }

      [ContextMenu("PrintGraphNeighbors")]
      public void PrintGraphNeighbors()
      {
         if(graph is null) return;

         var nodes = graph.GetNodes();
         foreach (var node in nodes)
         {
            node.PrintNeighbors();
         }
      }      
      
      // [ContextMenu("RecreateGraph")]
      public void RecreateGraph()
      {
         DestroyGraph();
         CreateGraph();
      }

      private void DestroyGraph()
      {
         if(graph is null) return;

         var nodes = graph.GetNodes();

         foreach (var node in nodes)
         {
            DestroyImmediate(node.gameObject);
         }
         
         graph = null;
      }

   }
}
