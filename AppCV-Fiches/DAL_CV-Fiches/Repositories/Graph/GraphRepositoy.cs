using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public abstract class GraphRepositoy<T> : IGraphRepository<T> where T : GraphObject, new()
    {
        protected DocumentClient documentClient;
        protected DocumentCollection documentCollection;

        protected string endpoint = "https://localhost:8081", primarykey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", database, graph;

        private bool AddHasEdges;
        private List<PostponedEdgeCreator> posponedCreateEdge;

        public GraphRepositoy(DocumentClient documentClient, DocumentCollection documentCollection)
        {
            this.documentClient = documentClient;
            this.documentCollection = documentCollection;
        }

        public GraphRepositoy(string Endpoint, string Key, string Database, string Graph)
        {
            //"Graphe_Essay"
            //"graph_cv"


            this.endpoint = Endpoint;
            this.primarykey = Key;
            this.database = Database;
            this.graph = Graph;

            documentClient = new DocumentClient(new Uri(Endpoint), Key);
            documentCollection = documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Database, Graph), new RequestOptions { OfferThroughput = 400 }).Result;
        }

        public GraphRepositoy(string Database, string Graph)
        {
            this.database = Database;
            this.graph = Graph;

            documentClient = new DocumentClient(new Uri(this.endpoint), this.primarykey);
            documentCollection = documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Database, Graph), new RequestOptions { OfferThroughput = 400 }).Result;
        }

        public void Add(T obj)
        {
            string addQuery = GetAddQuery(obj);
            obj.graphKey = ExecuteCommand(addQuery);

            if (AddHasEdges)
            {
                GraphObject item;

                foreach (PostponedEdgeCreator tuple in posponedCreateEdge)
                {
                    object genericRepository = GetGenericRepository(tuple.VertexType);

                    for (int i = 0; i < tuple.ListOfVertexes.Count; i++)
                    {
                        item = (GraphObject)tuple.ListOfVertexes[i];
                        if (string.IsNullOrEmpty(item.graphKey))
                            genericRepository.GetType().GetMethod("Add").Invoke(genericRepository, new object[] { item });

                        CreateEdge(obj, item, tuple.EdgeAttribute);
                    }
                }

                AddHasEdges = false;
                posponedCreateEdge.Clear();
                posponedCreateEdge = null;
            }
        }

        public void Delete(T obj)
        {
            string updateQuery = $"g.V('{obj.graphKey}').drop()";            
            ExecuteCommand(updateQuery);
        }

        public List<T> GetAll()
        {
            List<T> listOfObj = new List<T>();
            foreach (Vertex vertex in ExecuteCommandQueryVertex(GetGetAllQuery()))
            {
                listOfObj.Add(GetObjectFromVertex(vertex));
            }

            return listOfObj;
        }

        public T GetOne(string id)
        {
            List<T> listOfElements = new List<T>();
            string getOneQuery = $"g.V('{id}')";

            foreach (Vertex vertex in ExecuteCommandQueryVertex(getOneQuery))
                return GetObjectFromVertex(vertex);

            return null;
        }

        public List<T> Search(T searchObject)
        {
            List<T> listOfElements = new List<T>();

            Type thisType = searchObject.GetType();
            string searchQuery = $"g.V().hasLabel('{thisType.Name}')";

            object currentValue = null;

            foreach (PropertyInfo propInfo in thisType.GetProperties())
            {
                currentValue = propInfo.GetValue(searchObject);
                if (currentValue != null)
                {
                    if(propInfo.PropertyType.BaseType == typeof(Enum))
                        searchQuery += $".has('{propInfo.Name}','{Convert.ToInt32(currentValue)}')";
                    else
                        searchQuery += $".has('{propInfo.Name}','{currentValue}')";
                }
            }

            foreach (Vertex vertex in ExecuteCommandQueryVertex(searchQuery))
                listOfElements.Add(GetObjectFromVertex(vertex));

            return listOfElements;

        }

        public void Update(T obj)
        {
            Type thisType = obj.GetType();
            string updateQuery = $"g.V('{obj.graphKey}')";

            object currentValue = null;

            foreach (PropertyInfo propInfo in thisType.GetProperties())
            {
                currentValue = propInfo.GetValue(updateQuery);
                if (currentValue != null)
                {
                    if (propInfo.PropertyType.BaseType == typeof(Enum))
                        updateQuery += $".property('{propInfo.Name}','{Convert.ToInt32(currentValue)}')";
                    else
                        updateQuery += $".property('{propInfo.Name}','{currentValue}')";
                }
            }

            ExecuteCommand(updateQuery);
        }

        protected string GetAddQuery(T obj)
        {
            Type thisType = obj.GetType();
            string query = $"g.addV('{thisType.Name}')";

            Object value = null;
            foreach (var item in thisType.GetProperties())
            {
                value = item.GetValue(obj);

                if (value == null)
                    continue;

                EdgeProperty edgeProperty = null;

                if (!string.IsNullOrEmpty(value.ToString()))
                {

                    edgeProperty = (EdgeProperty)item.GetCustomAttribute(typeof(EdgeProperty));
                    if (edgeProperty != null)
                        continue;

                    if (value is String)
                        query += $".property(\"{item.Name}\", \"{value.ToString()}\")"; //Ajouter échap pour "
                    else if (value is Int32)
                        query += $".property('{item.Name}', {value})";
                    else if (value is Enum)
                        query += $".property('{item.Name}', {(int)value})";
                    else if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Attributes.Edge att = (Attributes.Edge)item.GetCustomAttribute(typeof(Attributes.Edge));
                        if (att != null)
                        {
                            if (posponedCreateEdge == null)
                                posponedCreateEdge = new List<PostponedEdgeCreator>();

                            posponedCreateEdge.Add(new PostponedEdgeCreator { EdgeAttribute = att, ListOfVertexes = (IList)value, VertexType = item.PropertyType.GetGenericArguments()[0] });
                            AddHasEdges = true;
                        }
                        else
                        {
                            //Embbeded object
                        }
                    }
                }
            }            

            return query;
        }

        protected string GetGetAllQuery()
        {
            Type thisType = typeof(T);
            string getAllQuery = $"g.V().hasLabel('{thisType.Name}')";

            return getAllQuery;
        }

        protected T GetObjectFromVertex(Vertex vertex)
        {
            Type thisType = typeof(T),
                 genericArgumentType = null;
            T genObj = new T();

            Attribute att;
            Attributes.Edge attEdge;

            Dictionary<string, object> vertexProperties = new Dictionary<string, object>();
            foreach (VertexProperty property in vertex.GetVertexProperties())
                vertexProperties.Add(property.Key, property.Value);

            genObj.graphKey = vertex.Id.ToString();
            object genericRepository = null;

            foreach (PropertyInfo prop in thisType.GetProperties())
            {
                att = prop.GetCustomAttribute(typeof(Attributes.Edge));
                if (att != null)
                {
                    attEdge = (Attributes.Edge)att;
                    genericArgumentType = prop.PropertyType.GetGenericArguments()[0];

                    genericRepository = GetGenericRepository(genericArgumentType);
                    prop.SetValue(genObj, genericRepository.GetType().GetMethod("GetElementsFromTransversal").Invoke(genericRepository, new object[] { genObj.graphKey, attEdge.EdgeName }));
                }
                else
                {
                    if (vertexProperties.ContainsKey(prop.Name))
                        prop.SetValue(genObj, vertexProperties[prop.Name]);
                }                    
            }          

            return genObj;
        }

        public List<T> GetElementsFromTransversal(string fromId, string edgeName)
        {
            List<T> listOfObjects = new List<T>();
            Type thisType = typeof(T);
            Attribute att;

            string transversalQuery = $"g.V('{fromId}').out('{edgeName}').hasLabel('{thisType.Name}')",
                getEdgeQuery = string.Empty;

            foreach (Vertex vertex in ExecuteCommandQueryVertex(transversalQuery))
            {
                T embeddedObject = GetObjectFromVertex(vertex);
                foreach (PropertyInfo propInfo in thisType.GetProperties())
                {
                    att = propInfo.GetCustomAttribute(typeof(EdgeProperty));
                    if (att != null)
                    {
                        getEdgeQuery = $"g.V('{fromId}').outE('{edgeName}').where(inV().has('id', '{embeddedObject.graphKey}'))";
                        foreach (Microsoft.Azure.Graphs.Elements.Edge edge in ExecuteCommandQueryEdge(getEdgeQuery))
                        {
                            if(propInfo.PropertyType.BaseType == typeof(Enum))
                                propInfo.SetValue(embeddedObject, Convert.ToInt32(edge.GetProperty(propInfo.Name).Value));
                            else
                                propInfo.SetValue(embeddedObject, edge.GetProperty(propInfo.Name).Value);
                        }
                    }
                }

                listOfObjects.Add(embeddedObject);
            }

            return listOfObjects;
        }

        private object GetGenericRepository(Type genericArgument)
        {
            Type thisType = this.GetType(), currentRepoType = null, interfaceType = null;
            Type[] typelist = GetTypesInNamespace(thisType.Assembly, thisType.Namespace),
                genericArguments = null;

            object genericRepository = null;

            for (int i = 0; i < typelist.Length; i++)
            {
                currentRepoType = typelist[i];
                interfaceType = currentRepoType.GetInterface("IGraphRepository`1");

                if (interfaceType != null)
                {
                    genericArguments = interfaceType.GetGenericArguments();
                    if (genericArguments != null && genericArguments.Length > 0)
                    {
                        if (genericArgument == genericArguments[0])
                        {
                            genericRepository = Activator.CreateInstance(currentRepoType, documentClient, documentCollection);
                        }
                    }
                }
            }

            return genericRepository;
        }

        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        private string ExecuteCommand(string CommandString)
        {
            //using (StreamWriter sw = new StreamWriter(@"C:\apache-tinkerpop-gremlin-console-3.3.3\customData\commandLog.log", true))
            //{
            //    sw.WriteLine(CommandString);
            //}

            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString);
            var feedResponse = query.ExecuteNextAsync<Vertex>().Result;
            return feedResponse.First().Id.ToString();
        }

        private FeedResponse<Vertex> ExecuteCommandQueryVertex(string CommandString)
        {
            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString);
            return query.ExecuteNextAsync<Vertex>().Result;
        }

        private FeedResponse<Microsoft.Azure.Graphs.Elements.Edge> ExecuteCommandQueryEdge(string CommandString)
        {
            IDocumentQuery<Microsoft.Azure.Graphs.Elements.Edge> query = documentClient.CreateGremlinQuery<Microsoft.Azure.Graphs.Elements.Edge>(documentCollection, CommandString);
            return query.ExecuteNextAsync<Microsoft.Azure.Graphs.Elements.Edge>().Result;
        }

        private FeedResponse<dynamic> ExecuteCommandQueryDynamic(string CommandString)
        {
            IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(documentCollection, CommandString);
            return query.ExecuteNextAsync<dynamic>().Result;
        }

        private void CreateEdge(GraphObject from, GraphObject to, Attributes.Edge EdgeAttribute)
        {
            string addEdge = $"g.V('{from.graphKey}').addE('{EdgeAttribute.EdgeName}').to(g.V('{to.graphKey}'))";

            if (!EdgeAttribute.IgnoreEdgeProperties)
            {
                Attribute edgeAtt;
                foreach (PropertyInfo prop in to.GetType().GetProperties())
                {
                    edgeAtt = prop.GetCustomAttribute(typeof(EdgeProperty));
                    if (edgeAtt != null)
                    {
                        if(prop.PropertyType.BaseType == typeof(Enum))
                            addEdge += $".property('{prop.Name}', '{(int)prop.GetValue(to)}')";
                        else if (prop.PropertyType == typeof(Int32))
                            addEdge += $".property('{prop.Name}', {prop.GetValue(to)})";
                        else
                            addEdge += $".property('{prop.Name}', '{prop.GetValue(to)}')";
                    }
                }
            }

            ExecuteCommand(addEdge);
        }

        public int CountEdges()
        {
            int numberOfEdges = 0;
            string countEdgesQuery = "g.E().count()";

            var result = ExecuteCommandQueryDynamic(countEdgesQuery);
            numberOfEdges = Convert.ToInt32(result.First());

            return numberOfEdges;
        }
    }

    internal struct PostponedEdgeCreator
    {
        internal Type VertexType { get; set; }
        internal Attributes.Edge EdgeAttribute { get; set; }
        internal IList ListOfVertexes { get; set; }
    }
}
