using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using Attribute;
using Systems.GraphSystem;
using UnityEngine;
using Utils.Extensions;
using Application = UnityEngine.Device.Application;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Systems.TileNodeSystem.Test
{
   public class TileNodeSystemTest : MonoBehaviour
   {
      [SerializeField] private GameObject nodePrefab;
      [SerializeField] private GameObject tileObjectPrefab;
      [SerializeField] private float nodeDistance = 1.5f;
      [SerializeField] private Vector2Int matrixDimensions = new Vector2Int(5, 5);
      [SerializeField] private bool createGraphInStart = true;
      [SerializeField] private bool drawGraphVertex = false;
      
      [Button(nameof(RecreateGraph))] public bool buttonField;

      private TileGraph tileGraph;
   
      private void Start()
      {
         if (createGraphInStart)
         {
            CreateGraph();
         }
      }

      private void CreateGraph()
      {
         tileGraph = new TileGraph();

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
               Vector3 localPos = new Vector3(i * nodeDistance, 0, j * nodeDistance);
               Vector3 pos = parentPos + localPos;
               
               int rndValue = Random.Range(0, 3);
               TileObject tileObject = null;
               
               if (rndValue > 0)
               {
                  tileObject = CreateTileObject(rndValue, pos);
               }
               
               Debug.Log("rndValue : " + rndValue);
               TileNodeController tileNodeController = CreateNode(tileObject, pos);
               ++nodeCount;
               tileNodeController.gameObject.name = "Node_" + nodeCount;

               tileGraph.AddNode(tileNodeController.TileNode);
            }
         }
      }

      private void FindEdges()
      {
         tileGraph.FindEdgesWithNodeDistance(nodeDistance, drawGraphVertex);
         // tileGraph.PrintGraphNeighbors();
      }

      #region Test Objects Parent

      private GameObject NodesParent
      {
         get
         {
            if (GameUtility.IsNull(nodesParent))
            {
               nodesParent = new GameObject
               {
                  name = "NodesParent",
                  transform = { parent = transform }
               };
            }
            return nodesParent;
         }
      }
      private GameObject nodesParent;
      
      private GameObject TileObjectsParent
      {
         get
         {
            if (GameUtility.IsNull(tileObjectsParent))
            {
               tileObjectsParent = new GameObject
               {
                  name = "TileObjectsParent",
                  transform = { parent = transform }
               };
            }
            return tileObjectsParent;
         }
      }
      private GameObject tileObjectsParent;

      #endregion

      private TileNodeController CreateNode(TileObject tileObject, Vector3 pos)
      {
         var nodeObject = Instantiate(nodePrefab, pos, Quaternion.identity, NodesParent.transform);
         var tileNodeController = nodeObject.GetComponent<TileNodeController>();
         tileNodeController.Init(tileObject);
         return tileNodeController;
      }

      public TileObject CreateTileObject(int objectId, Vector3 pos)
      {
         var tileObjectGo = Instantiate(tileObjectPrefab, pos, Quaternion.identity, TileObjectsParent.transform);
         var tileObject = tileObjectGo.GetComponent<TileObject>();
         tileObject.Init(new TileObjectValue(objectId, 1));
         return tileObject;
      }

      [ContextMenu("PrintGraphNeighbors")]
      public void PrintGraphNeighbors()
      {
         tileGraph?.PrintGraphNeighbors();
      }      
      
      // [ContextMenu("RecreateGraph")]
      public void RecreateGraph()
      {
         DestroyGraph();
         DestroyTileObjects();
         CreateGraph();
      }

      private void DestroyTileObjects()
      {
         if (Application.isPlaying)
         {
            TileObjectsParent.transform.DestroyChildren();
         }
         else
         {
            TileObjectsParent.transform.DestroyImmediateChildren();
         }
      }

      private void DestroyGraph()
      {
         if (Application.isPlaying)
         {
            tileGraph?.DestroyNodes();
         }
         else
         {
            tileGraph?.DestroyImmediateNodes();
         }
         tileGraph = null;
      }
   }
}
