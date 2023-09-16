

using System;
using System.Collections.Generic;
using System.Data.Common;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using Attribute;
using Systems.GraphSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.Extensions;

public class TileNode : Node<TileNode, TileObjectValue>
{
    public Action<TileObjectValue> onTileObjectMerged; 
        
    [Button(nameof(Bfs))] public bool buttonField;

    private const int MAX_NEIGHBORS_COUNT = 4;

    #region Init Functions

    public override void Init(TileObjectValue value)
    {
        base.Init(value);
    }

    #endregion

    #region Other Functions

    [ContextMenu("Bfs")]
    public void Bfs()
    {
        var wantedNodes = TileGraph.FindWantedNodesWithBfs(this, Value );
        
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

[Serializable]
public class ValueModelPair
{
    public int value;
    public Mesh model;
}
