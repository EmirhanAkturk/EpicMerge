using UnityEngine;

namespace Systems.GraphSystem
{
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
}
