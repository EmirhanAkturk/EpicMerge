
using System.Collections.Generic;

public interface INode
{
    public bool AddNeighbor(INode neighbor);
    public bool IsNeighbor(INode node);
    public List<INode> GetNeighbors(INode node);
}
