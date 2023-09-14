

using System;
using System.Collections.Generic;
using System.Data.Common;
using Attribute;
using Systems.GraphSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class TileNode : Node
{
    [SerializeField] private MeshRenderer meshRenderer;
    [Space]
    [SerializeField] private List<ValueMaterialPair> valueMaterialPairs;
    
    [Space]
    [Button(nameof(Bfs))] public bool buttonField;

    private Dictionary<int, ValueMaterialPair> ValueMaterialMaps
    {
        get
        {
            if (valueMaterialMaps is null)
            {
                InitValueMaterialMaps();
            }

            return valueMaterialMaps;
        }
    }
    private Dictionary<int, ValueMaterialPair> valueMaterialMaps;

    private const int MAX_NEIGHBORS_COUNT = 4;

    #region Init Functions

    public override void Init(int value)
    {
        base.Init(value);
        InitVisual();
    }

    private void InitVisual()
    {
        var valueMaterialPair = GetValueMaterialPair(Value);
        SetMaterial(valueMaterialPair.material);
    }

    #endregion

    #region Set Functions

    private void SetMaterial(Material material)
    {
        if(material is null) return;
        meshRenderer.material = material;
    }


    #endregion
    
    #region Get Functions

    private ValueMaterialPair GetValueMaterialPair(int value)
    {
        return ValueMaterialMaps.TryGetValue(value, out var valueMaterialPair) ? valueMaterialPair : null;
    }

    #endregion

    #region Other Functions

    [ContextMenu("Bfs")]
    public void Bfs()
    {
        var wantedNodes = Graph.FindWantedNodesWithBfs(this, Value);
        
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
            GameObject nodeObject = node.gameObject;
            Debug.Log("Nodes order : " + nodesOrder + "NodeName : " + nodeObject.name, nodeObject);
        }
    }

    protected override bool CheckNeighborsFull()
    {
        return Neighbors.Count >= MAX_NEIGHBORS_COUNT;
    }
    
    private void InitValueMaterialMaps()
    {
        valueMaterialMaps ??= new Dictionary<int, ValueMaterialPair>();

        foreach (var pair in valueMaterialPairs)
        {
            valueMaterialMaps.Add(pair.value, pair);
        }
    }
    
    #endregion
}

[Serializable]
public class ValueMaterialPair
{
    public int value;
    public Material material;
}