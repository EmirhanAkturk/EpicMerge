using System.Collections;
using System.Collections.Generic;
using Systems.GraphSystem;
using UnityEngine;
using Utils;

public class IntNode : Node<IntNode, int> 
{
    public override void PrintNeighborsValues()
    {
        Debug.Log("#######################");
        Debug.Log("Print My Neighbors Value: " + Value);

        PrintNodeValues(Neighbors);
    }

    public IntNode(int value) : base(value)
    {
        
    }
}
