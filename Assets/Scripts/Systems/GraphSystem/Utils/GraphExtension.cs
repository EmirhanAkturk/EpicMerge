namespace Systems.GraphSystem.Utils
{
    public static class GraphExtension 
    {
        public static void PrintGraphNeighbors<T, TF>(this Graph<T, TF> graph) where T : Node<T, TF>
        {
            var nodes = graph.GetNodes();
            foreach (var node in nodes)
            {
                node.PrintNeighborsValues();
            }    
        }
    }
}
