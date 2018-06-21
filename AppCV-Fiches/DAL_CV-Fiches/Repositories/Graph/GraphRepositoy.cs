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
using Type = System.Type;

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
            obj.GraphKey = ExecuteCommandVertex(addQuery);

            if (AddHasEdges)
            {
                GraphObject item;

                foreach (PostponedEdgeCreator tuple in posponedCreateEdge)
                {
                    object genericRepository = GetGenericRepository(tuple.VertexType);

                    for (int i = 0; i < tuple.ListOfVertexes.Count; i++)
                    {
                        item = (GraphObject)tuple.ListOfVertexes[i];
                        if (string.IsNullOrEmpty(item.GraphKey))
                            genericRepository.GetType().GetMethod("Add").Invoke(genericRepository, new object[] { item });
                        //else
                        //    genericRepository.GetType().GetMethod("Update").Invoke(genericRepository, new object[] { item });

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
            string updateQuery = $"g.V('{obj.GraphKey}').drop()";            
            ExecuteCommandVertex(updateQuery);
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

            Attribute edgeAtt;
            Type thisType = searchObject.GetType();
            string searchQuery = $"g.V().hasLabel('{thisType.Name}')";

            object currentValue = null;

            foreach (PropertyInfo propInfo in thisType.GetProperties())
            {
                edgeAtt = propInfo.GetCustomAttribute(typeof(EdgeProperty));
                if (edgeAtt != null)
                    continue;

                currentValue = propInfo.GetValue(searchObject);
                if (currentValue != null)
                {
                    if (propInfo.PropertyType.BaseType == typeof(Enum))
                        searchQuery += $".has('{propInfo.Name}','{Convert.ToInt32(currentValue)}')";
                    else
                        searchQuery += $".has('{propInfo.Name}','{currentValue.ToString().Replace("'", "’")}')";
                }
            }

            foreach (Vertex vertex in ExecuteCommandQueryVertex(searchQuery))
                listOfElements.Add(GetObjectFromVertex(vertex));

            return listOfElements;
        }

        public List<T> Search(T searchObject, bool returnDefault)
        {
            List<T> listOfElements;
            listOfElements = Search(searchObject);

            if (listOfElements.Count == 0 && returnDefault)
                listOfElements.Add(searchObject);

            return listOfElements;
        }

        public List<T> Search(Dictionary<string, object> properties)
        {
            List<T> listOfElements = new List<T>();

            Type thisType = typeof(T);
            string searchQuery = $"g.V().hasLabel('{thisType.Name}')";

            foreach (KeyValuePair<string, object> property in properties)
            {
                if (property.Key == null || property.Value == null)
                    continue;

                if (property.Value.GetType().BaseType == typeof(Enum))
                    searchQuery += $".has(\"{property}\",{Convert.ToInt32(property.Value)})";
                else
                    searchQuery += $".has(\"{property.Key}\",\"{property.Value.ToString().Replace("'", "’")}\")";
            }          

            foreach (Vertex vertex in ExecuteCommandQueryVertex(searchQuery))
                listOfElements.Add(GetObjectFromVertex(vertex));

            return listOfElements;
        }

        public List<T> Search(Dictionary<string, object> properties, bool returnDefault)
        {
            List<T> listOfElements;
            listOfElements = Search(properties);

            if (listOfElements.Count == 0 && returnDefault)
            {
                T obj = new T();
                Type thisType = obj.GetType();

                foreach (KeyValuePair<string, object> property in properties)
                {
                    if (property.Key == null || property.Value == null)
                        continue;

                    thisType.GetProperty(property.Key).SetValue(obj, property.Value);
                }                    

                listOfElements.Add(obj);
            }                

            return listOfElements;
        }

        public void Update(T obj)
        {
            Type thisType = obj.GetType();
            string updateQuery = $"g.V('{obj.GraphKey}')";

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

            ExecuteCommandVertex(updateQuery);
        }

        public T CreateIfNotExists(T obj)
        {
            obj = Search(obj, true).First();
            if (string.IsNullOrEmpty(obj.GraphKey))
                Add(obj);

            return obj;
        }

        public T CreateIfNotExists(Dictionary<string, object> properties)
        {
            T obj;

            obj = Search(properties, true).First();
            if (string.IsNullOrEmpty(obj.GraphKey))
                Add(obj);

            return obj;
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
                        query += $".property(\"{item.Name}\", \"{value.ToString().Replace("'", "’")}\")"; //Ajouter échap pour "
                    else if (value is Int32)
                        query += $".property(\"{item.Name}\", {value})";
                    else if (value is Enum)
                        query += $".property(\"{item.Name}\", {(int)value})";
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
                    else if (value is DateTime)
                        query += $".property(\"{item.Name}\", \"{((DateTime)value).ToString()}\")";
                    else if (value is GraphObject)
                    {
                        Attributes.Edge att = (Attributes.Edge)item.GetCustomAttribute(typeof(Attributes.Edge));
                        if (att != null)
                        {
                            if (posponedCreateEdge == null)
                                posponedCreateEdge = new List<PostponedEdgeCreator>();

                            List<object> list = new List<object>();
                            list.Add(value);

                            posponedCreateEdge.Add(new PostponedEdgeCreator { EdgeAttribute = att, ListOfVertexes = (IList)list, VertexType = item.PropertyType });
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
                 genericArgumentType = null,
                 interfaceType = null;
            T genObj = new T();

            Attribute att;
            Attributes.Edge attEdge;

            Dictionary<string, object> vertexProperties = new Dictionary<string, object>();
            foreach (VertexProperty property in vertex.GetVertexProperties())
                vertexProperties.Add(property.Key, property.Value);

            genObj.GraphKey = vertex.Id.ToString();
            object genericRepository = null;

            foreach (PropertyInfo prop in thisType.GetProperties())
            {
                att = prop.GetCustomAttribute(typeof(Attributes.Edge));
                if (att != null)
                {
                    attEdge = (Attributes.Edge)att;

                    interfaceType = prop.PropertyType.GetInterface("IList`1");
                    if (interfaceType != null)
                    {
                        genericArgumentType = prop.PropertyType.GetGenericArguments()[0];
                        genericRepository = GetGenericRepository(genericArgumentType);
                        prop.SetValue(genObj, genericRepository.GetType().GetMethod("GetElementsFromTransversal").Invoke(genericRepository, new object[] { genObj.GraphKey, attEdge.EdgeName }));
                    }
                    else
                    {
                        genericRepository = GetGenericRepository(prop.PropertyType);
                        prop.SetValue(genObj, genericRepository.GetType().GetMethod("GetElementFromTransversal").Invoke(genericRepository, new object[] { genObj.GraphKey, attEdge.EdgeName }));
                    }
                }
                else
                {
                    if (vertexProperties.ContainsKey(prop.Name))
                        prop.SetValue(genObj, Convert.ChangeType(vertexProperties[prop.Name], prop.PropertyType));
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
                        getEdgeQuery = $"g.V('{fromId}').outE('{edgeName}').where(inV().has('id', '{embeddedObject.GraphKey}'))";
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

        public T GetElementFromTransversal(string fromId, string edgeName)
        {
            return GetElementsFromTransversal(fromId, edgeName).DefaultIfEmpty(null).FirstOrDefault();
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

        private void ExecuteCommandEdge(string CommandString)
        {
            IDocumentQuery<Microsoft.Azure.Graphs.Elements.Edge> query = documentClient.CreateGremlinQuery<Microsoft.Azure.Graphs.Elements.Edge>(documentCollection, CommandString);
            var feedResponse = query.ExecuteNextAsync<Microsoft.Azure.Graphs.Elements.Edge>().Result;
        }

        private string ExecuteCommandVertex(string CommandString) 
        {
            //using (StreamWriter sw = new StreamWriter(@"C:\apache-tinkerpop-gremlin-console-3.3.3\customData\commandLog.log", true))
            //{
            //    sw.WriteLine(CommandString);
            //}

            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString.Replace("$", "\\$"));
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
            string addEdge = $"g.V('{from.GraphKey}').addE('{EdgeAttribute.EdgeName}').to(g.V('{to.GraphKey}'))",
                valueToString = string.Empty;

            object valueToObject = null;

            if (!EdgeAttribute.IgnoreEdgeProperties)
            {
                Attribute edgeAtt;
                foreach (PropertyInfo prop in to.GetType().GetProperties())
                {
                    edgeAtt = prop.GetCustomAttribute(typeof(EdgeProperty));
                    if (edgeAtt != null)
                    {
                        if(prop.PropertyType.BaseType == typeof(Enum))
                            addEdge += $".property(\"{prop.Name}\", {(int)prop.GetValue(to)})";
                        else if (prop.PropertyType == typeof(Int32))
                            addEdge += $".property(\"{prop.Name}\", {prop.GetValue(to)})";
                        else if (prop.PropertyType == typeof(DateTime))
                            addEdge += $".property(\"{prop.Name}\", \"{((DateTime)prop.GetValue(to)).ToString()}\")";
                        else
                        {
                            valueToObject = prop.GetValue(to);
                            if (valueToObject != null)
                            {
                                valueToString = valueToObject.ToString().Replace("'", "’");
                                addEdge += $".property(\"{prop.Name}\", \"{valueToString}\")";
                            }
                            else
                                addEdge += $".property(\"{prop.Name}\", \"{valueToObject}\")";


                        }
                            
                    }
                }
            }

            ExecuteCommandEdge(addEdge);
        }

        private bool HasEdge(Attributes.Edge edge, GraphObject from, GraphObject to)
        {

            //string findEdgeQuery = $"g().E("")"
            return false;
        }

        public int CountEdges()
        {
            int numberOfEdges = 0;
            string countEdgesQuery = "g.E().count()";

            var result = ExecuteCommandQueryDynamic(countEdgesQuery);
            numberOfEdges = Convert.ToInt32(result.First());

            return numberOfEdges;
        }

        public void DeleteAllDocs()
        {
            var a = documentClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("Graphe_Essay", "graph_cv")).Result;
            var b = documentClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri("Graphe_Essay"), new DocumentCollection { Id = "graph_cv" }).Result;
        }
    }

    internal struct PostponedEdgeCreator
    {
        internal Type VertexType { get; set; }
        internal Attributes.Edge EdgeAttribute { get; set; }
        internal IList ListOfVertexes { get; set; }
    }
}
