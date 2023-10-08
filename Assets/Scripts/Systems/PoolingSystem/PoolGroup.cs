using System;
using System.Collections.Generic;

namespace Systems.PoolingSystem
{
    [Serializable]
    public class PoolGroup
    {
        public string groupName;
        public string groupPath;
        public List<PoolElement> PoolElements;
    }
}
