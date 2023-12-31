using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using Attribute;
using GameDepends.Enums;
using Systems.ConfigurationSystem;
using Systems.PoolingSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Extensions;
using Zenject;
using Application = UnityEngine.Device.Application;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Systems.TileNodeSystem.GraphGenerator
{
   public class TileGraphGenerator : MonoBehaviour
   {
      public int GraphGraphGeneratorId => graphGeneratorId;
      public string GraphGeneratorName => graphGeneratorName;
      
      [SerializeField] private int graphGeneratorId;
      [SerializeField] private string graphGeneratorName;
      
      [Space]
      [SerializeField] private PoolType nodeType;
      [SerializeField] private PoolType tileObjectType;

      [Space]
      [SerializeField] private float nodeDistance = 1.5f;
      [SerializeField] private Vector2Int matrixDimensions = new Vector2Int(5, 5);
      [SerializeField] private bool createGraphInStart = true;
      // [SerializeField] private bool drawGraphVertex = false;
      [Range(0f, 100f)][SerializeField] private float nodeCanDeleteRatio = 20f;
      
      [Button(nameof(RecreateGraph))] public bool buttonField;

      private readonly List<GameObject> nodeObjects = new List<GameObject>();
      private readonly List<GameObject> tileObjects = new List<GameObject>();
      private TileGraph tileGraph;
   
      private void Start()
      {
         if (createGraphInStart)
         {
            CreateGraph();
         }
      }

      private void OnEnable()
      {
         TileGraphGeneratorManager.Instance.AddGenerator(this);
      }

      private void OnDisable()
      {
         if (TileGraphGeneratorManager.IsAvailable())
         {
            TileGraphGeneratorManager.Instance.RemoveGenerator(this);
         }
      }

      public void CreateGraph()
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
         StringBuilder nodeName = new StringBuilder();

         int mergeableObjectTypeCount = ConfigurationService.Configurations.mergeableObjectTypeCount;
         
         for (int i = 0; i < m; i++)
         {
            for (int j = 0; j < n; j++)
            {
               if (Random.Range(0, 100) < nodeCanDeleteRatio)
               {
                  // Dont Create This node
                  continue;
               }
               
               Vector3 localPos = new Vector3(i * nodeDistance, 0, j * nodeDistance);
               Vector3 pos = parentPos + localPos;
               
               int rndValue = Random.Range(0, mergeableObjectTypeCount + 1);
               BaseTileObject baseTileObject = null;
               
               if (rndValue > 0)
               {
                  baseTileObject = CreateTileObject(rndValue, pos);
               }
               
               nodeName.Clear();
               nodeName.Append("Node_");
               nodeName.Append(nodeCount);
               
               TileNode tileNode = CreateNode(baseTileObject, pos, nodeName.ToString());
               ++nodeCount;

               tileGraph.AddNode( tileNode);
            }
         }
      }

      private void FindEdges()
      {
         tileGraph.FindEdgesWithNodeDistance(nodeDistance/*, drawGraphVertex*/);
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

      private TileNode CreateNode(BaseTileObject baseTileObject, Vector3 pos, string nodeName)
      {
         var tileNodeController = PoolingSystem.Instance.Create<TileNodeController>(nodeType, NodesParent.transform);
         
         var nodeObject = tileNodeController.gameObject;
         var nodeObjectTr = nodeObject.transform;
         
         nodeObjectTr.position = pos;
         nodeObjectTr.rotation = Quaternion.identity;
         nodeObject.name = nodeName;
         
         TileObjectValue tileObjectValue = baseTileObject != null ? baseTileObject.TileObjectValue : TileObjectValue.GetEmptyTileObjectValue(); 
         TileNode tileNode = new TileNode(nodeObject.transform, tileObjectValue);
         
         tileNodeController.Init(tileNode, baseTileObject);
         
         nodeObjects.Add(nodeObject);
         return tileNode;
      }

      public BaseTileObject CreateTileObject(int objectId, Vector3 pos)
      {
         var tileObject = PoolingSystem.Instance.Create<BaseTileObject>(tileObjectType, TileObjectsParent.transform);
         
         var tileObjectGo = tileObject.gameObject;
         var tileObjectTr = tileObjectGo.transform;
         
         tileObjectTr.position = pos;
         tileObjectTr.rotation = Quaternion.identity;
         
         tileObject.Init(new TileObjectValue(objectId, 1));

         tileObjects.Add(tileObjectGo);
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
            foreach (var tileObject in tileObjects)
            {
               // TODO Fix Pooling Bug
               // PoolingSystem.Instance.Destroy(tileObjectType, tileObject, false);
               Destroy(tileObject);
            }
            tileObjects.Clear();
         }
         // else
         // {
         //    TileObjectsParent.transform.DestroyImmediateChildren();
         // }
      }

      private void DestroyGraph()
      {
         if (Application.isPlaying)
         {
            foreach (var nodeObject in nodeObjects)
            {
               // TODO Fix Pooling Bug
               // PoolingSystem.Instance.Destroy(nodeType, nodeObject, false);
               Destroy(nodeObject);
            }
            nodeObjects.Clear();
            tileGraph?.ClearNodes();
         }
         // else
         // {
         //    tileGraph?.DestroyImmediateNodes();
         // }
         tileGraph = null;
      }
      
   }
}
