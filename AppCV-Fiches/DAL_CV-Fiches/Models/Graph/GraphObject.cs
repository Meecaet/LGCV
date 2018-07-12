using DAL_CV_Fiches.Repositories.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace DAL_CV_Fiches.Models.Graph
{
    [Serializable]
    public abstract class GraphObject
    {
        protected Dictionary<string, object> InternalPropertyDictionary;

        public GraphObject(bool IsUpdateable)
        {
            this.IsUpdateable = IsUpdateable;
            InternalPropertyDictionary = new Dictionary<string, object>();
        }

        public GraphObject()
        {
            this.IsUpdateable = false;
            InternalPropertyDictionary = new Dictionary<string, object>();
        }

        [XmlIgnore]
        public string GraphKey { get; set; }
        internal bool IsUpdateable;

        [Edge("EteModifie", lazyLoad: true)]
        public List<EditionObject> EditionObjects {get => (List<EditionObject>)LoadProperty("EditionObjects"); set => SetProperty("EditionObjects", value); }

        public GraphObject GetPreviousVersion()
        {
            return null;
        }

        protected object LoadProperty(string propertyName)
        {
            if (InternalPropertyDictionary.ContainsKey(propertyName))
                return InternalPropertyDictionary[propertyName];

            Type propertyType, genericArgument;
            object genericRepository, propertyValue;

            PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);            
            Edge edgeAttr = propertyInfo.GetCustomAttribute<Edge>();

            propertyValue = null;
           
            if (edgeAttr != null)
            {
                propertyType = propertyInfo.PropertyType.UnderlyingSystemType;

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    genericArgument = propertyType.GetGenericArguments()[0];
                    genericRepository = GraphRepositoy.GetGenericRepository(genericArgument);

                    propertyValue = genericRepository.GetType().GetMethod("GetElementsFromTransversal").Invoke(genericRepository, new object[] { this.GraphKey, edgeAttr.EdgeName });
                }
                else
                {
                    genericRepository = GraphRepositoy.GetGenericRepository(propertyType);

                    propertyValue = genericRepository.GetType().GetMethod("GetElementFromTransversal").Invoke(genericRepository, new object[] { this.GraphKey, edgeAttr.EdgeName });
                }

                SetProperty(propertyName, propertyValue);
            }

            return propertyValue;
        }

        protected void SetProperty(string propertyName, object value)
        {            
            if (InternalPropertyDictionary.ContainsKey(propertyName))
                InternalPropertyDictionary[propertyName] = value;
            else
                InternalPropertyDictionary.Add(propertyName, value);
        }
    }
}
