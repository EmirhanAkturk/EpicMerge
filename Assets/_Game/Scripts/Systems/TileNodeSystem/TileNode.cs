

using System;
using System.Collections.Generic;
using System.Data.Common;
using Attribute;
using Systems.GraphSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.Extensions;

public class TileNode : Node
{
    [Button(nameof(Bfs))] public bool buttonField;

    private const int MAX_NEIGHBORS_COUNT = 4;

    #region Init Functions

    public override void Init(int value)
    {
        base.Init(value);
    }

    #endregion

    #region Other Functions

    [ContextMenu("Bfs")]
    public void Bfs()
    {
        List<Node> wantedNodes = Graph.FindWantedNodesWithBfs(this, Value);
        
        Debug.Log("################## ");
        Debug.Log("BFS Wanted Value : " + Value);
        PrintNodes(wantedNodes);
    }
    
    public override void PrintNeighbors()
    {
        Debug.Log("#######################");
        Debug.Log("Print My Neighbors : ", gameObject);

        PrintNodes(Neighbors);
    }

    protected void PrintNodes(List<Node> nodes)
    {
        int nodesOrder = 0;
        foreach (Node node in nodes)
        {
            ++nodesOrder;
            GameObject nodeObject = node.gameObject;
            LogUtility.PrintLog("Nodes order : " + nodesOrder + ", NodeName : " + nodeObject.name, nodeObject);
        }
    }

    protected override bool CheckNeighborsFull()
    {
        return Neighbors.Count >= MAX_NEIGHBORS_COUNT;
    }
    
    #endregion
}

[Serializable]
public class ValueMaterialPair
{
    public int value;
    public Material material;
}
