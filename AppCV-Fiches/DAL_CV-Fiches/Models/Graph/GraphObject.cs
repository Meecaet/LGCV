using DAL_CV_Fiches.Repositories.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public abstract class GraphObject
    {
        public GraphObject(bool IsUpdateable)
        {
            this.IsUpdateable = IsUpdateable;
        }

        public GraphObject()
        {
            this.IsUpdateable = false;
        }

        [XmlIgnore]
        public string GraphKey { get; set; }
        internal bool IsUpdateable;

        public GraphObject GetPreviousVersion(IGraphRepository<GraphObject> repository)
        {
            return null;
        }

    }
}
