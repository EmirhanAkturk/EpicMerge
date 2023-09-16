using System.Collections;
using System.Collections.Generic;
using Systems.GraphSystem;
using UnityEngine;
using Utils;

public class IntNode : Node<IntNode, int> 
{
    public override void PrintNeighbors()
    {
        Debug.Log("#######################");
        Debug.Log("Print My Neighbors : ", gameObject);

        PrintNodes(Neighbors);
    }
}
