using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph.Attributes
{
    public class Edge : Attribute
    {
        public string EdgeName { get; }
        public bool IgnoreEdgeProperties { get; }

        public Edge(string edgeName, bool ignoreEdgeProperties = false)
        {
            this.EdgeName = edgeName;
            this.IgnoreEdgeProperties = ignoreEdgeProperties;
        }
    }
}
