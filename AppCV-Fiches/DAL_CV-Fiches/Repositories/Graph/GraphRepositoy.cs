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

            documentClient = GraphConfig.DocumentClient;
            documentCollection = GraphConfig.DocumentCollection;

            //documentClient = new DocumentClient(new Uri(Endpoint), Key);
            //documentCollection = documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Database, Graph), new RequestOptions { OfferThroughput = 400 }).Result;
        }

        public GraphRepositoy(string Database, string Graph)
        {
            this.database = Database;
            this.graph = Graph;

            documentClient = new DocumentClient(new Uri(this.endpoint), this.primarykey);
            documentCollection = documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Database, Graph), new RequestOptions { OfferThroughput = 400 }).Result;
        }

        public GraphRepositoy()
        {
            documentClient = GraphConfig.DocumentClient;
            documentCollection = GraphConfig.DocumentCollection;
        }

        public void Add(T obj, bool cascade = true)
        {
            string addQuery;
            object currentPropValue;
            Type thisType = obj.GetType();
            Attributes.Edge att;

            if (string.IsNullOrEmpty(obj.GraphKey))
            {
                addQuery = GetAddQuery(obj);
                obj.GraphKey = ExecuteCommandVertex(addQuery);
            }

            if (!cascade)
                return;

            foreach (PropertyInfo propInfo in thisType.GetProperties())
            {
                currentPropValue = propInfo.GetValue(obj);
                if (currentPropValue == null)
                    continue;

                if (propInfo.PropertyType.BaseType == typeof(GraphObject) || propInfo.PropertyType == typeof(GraphObject))
                {
                    att = (Attributes.Edge)propInfo.GetCustomAttribute(typeof(Attributes.Edge));

                    if (att == null)
                        throw new InvalidOperationException($"Relations between GraphObjects {thisType.Name} and {currentPropValue.GetType().Name} must have an Edge attribute");

                    CreateRelationIfNotExists(obj, att, (GraphObject)currentPropValue);
                }
                else if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    att = (Attributes.Edge)propInfo.GetCustomAttribute(typeof(Attributes.Edge));

                    if (att == null)
                        throw new InvalidOperationException($"Relations between GraphObjects {thisType.Name} and {currentPropValue.GetType().Name} must have an Edge attribute");

                    IList listOfGraphObjects = (IList)propInfo.GetValue(obj);
                    foreach (object graphObject in listOfGraphObjects)
                    {
                        CreateRelationIfNotExists(obj, att, (GraphObject)graphObject);
                    }
                }
            }
        }

        private void CreateRelationIfNotExists(GraphObject from, Attributes.Edge att, GraphObject to)
        {
            object genericRepository;

            genericRepository = GetGenericRepository(to.GetType());

            genericRepository.GetType().GetMethod("Add").Invoke(genericRepository, new object[] { to, true });

            if (!HasEdge(att, from, to))
                CreateEdge(from, to, att);
        }

        public virtual void Delete(T obj)
        {
            string updateQuery = $"g.V('{obj.GraphKey}').drop()";
            ExecuteCommandQueryVertex(updateQuery);
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
            string getOneQuery = $"g.V(\"{id}\")";

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
                    if (propInfo.PropertyType.BaseType == typeof(Enum) || propInfo.PropertyType.UnderlyingSystemType == typeof(Int32))
                        searchQuery += $".has(\"{propInfo.Name}\",{Convert.ToInt32(currentValue)})";
                    else
                        searchQuery += $".has(\"{propInfo.Name}\",\"{currentValue.ToString().Replace("'", "’")}\")";
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

                if (property.Value.GetType().BaseType == typeof(Enum) || property.Value.GetType() == typeof(Int32))
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
            object currentValue;

            foreach (PropertyInfo propInfo in thisType.GetProperties())
            {
                if (propInfo.Name == "GraphKey")
                    continue;

                currentValue = propInfo.GetValue(obj);
                if (currentValue != null)
                {
                    var edgeProperty = (EdgeProperty)thisType.GetCustomAttribute(typeof(EdgeProperty));
                    if (edgeProperty != null)
                        continue;

                    if (currentValue is String)
                        updateQuery += $".property(\"{propInfo.Name}\", \"{currentValue.ToString().Replace("'", "’")}\")";
                    else if (currentValue is Int32)
                        updateQuery += $".property(\"{propInfo.Name}\", {currentValue})";
                    else if (currentValue is Enum)
                        updateQuery += $".property(\"{propInfo.Name}\", {(int)currentValue})";
                    else if (currentValue is DateTime)
                        updateQuery += $".property(\"{propInfo.Name}\", \"{Convert.ToInt32(currentValue)}\")";
                }
                else
                {
                    updateQuery += $".property('{propInfo.Name}','')";
                }
            }

            ExecuteCommandVertex(updateQuery);

            //if (obj.IsUpdateable)
            //{
            //    //Update stardart
            //}
            //else
            //{

            //}

            //Type thisType = obj.GetType();
            //string updateQuery = $"g.V('{obj.GraphKey}')";

            //object currentValue = null, genericRepository = null;
            //Attributes.Edge att;
            //GraphObject graphObjectCurrentValue;

            //foreach (PropertyInfo propInfo in thisType.GetProperties())
            //{
            //    currentValue = propInfo.GetValue(updateQuery);
            //    if (currentValue != null)
            //    {
            //        if (propInfo.PropertyType.BaseType == typeof(Enum))
            //            updateQuery += $".property('{propInfo.Name}','{Convert.ToInt32(currentValue)}')";
            //        else if (currentValue is GraphObject)
            //        {
            //            graphObjectCurrentValue = (GraphObject)currentValue;
            //            att = (Attributes.Edge)currentValue.GetType().GetCustomAttribute(typeof(Attributes.Edge));
            //            if (att == null)
            //                continue;

            //            genericRepository = GetGenericRepository(currentValue.GetType());
            //            if (HasEdge(att, obj, graphObjectCurrentValue))
            //                genericRepository.GetType().GetMethod("Update").Invoke(genericRepository, new object[] { currentValue });
            //            else if (string.IsNullOrEmpty(graphObjectCurrentValue.GraphKey))
            //            {
            //                genericRepository.GetType().GetMethod("Add").Invoke(genericRepository, new object[] { currentValue });
            //                CreateEdge(obj, graphObjectCurrentValue, att);
            //            }

            //        }
            //        else
            //            updateQuery += $".property('{propInfo.Name}','{currentValue}')";
            //    }
            //}

            //ExecuteCommandVertex(updateQuery);
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
            string query = $"g.addV(\"{thisType.Name}\")";

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
                    else if (value is DateTime)
                        query += $".property(\"{item.Name}\", \"{((DateTime)value).ToString()}\")";
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

        protected T GetObjectFromVertex(Vertex vertex, Type noeudType = null)
        {
            Type thisType = noeudType != null ? noeudType : typeof(T),
                 genericArgumentType = null,
                 interfaceType = null;
            T genObj = (T)Activator.CreateInstance(thisType);

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
                    if (attEdge.LazyLoad)
                        continue;

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
                        if (prop.PropertyType.BaseType == typeof(Enum))
                            prop.SetValue(genObj, Convert.ToInt32(vertexProperties[prop.Name]));
                        else
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

            object edgePropertyValue;
            List<Property> edgeProperties;

            string transversalQuery = $"g.V('{fromId}').out('{edgeName}')";
            string getEdgeQuery = string.Empty;

            if (thisType != typeof(GraphObject))
            {
                transversalQuery = transversalQuery + $".hasLabel('{thisType.Name}')";
            }

            foreach (Vertex vertex in ExecuteCommandQueryVertex(transversalQuery))
            {
                if (thisType == typeof(GraphObject))
                {
                    thisType = Type.GetType($"DAL_CV_Fiches.Models.Graph.{vertex.Label}");
                }

                T embeddedObject = GetObjectFromVertex(vertex, thisType);

                foreach (PropertyInfo propInfo in thisType.GetProperties())
                {
                    att = propInfo.GetCustomAttribute(typeof(EdgeProperty));
                    if (att != null)
                    {
                        getEdgeQuery = $"g.V('{fromId}').outE('{edgeName}').where(inV().has('id', '{embeddedObject.GraphKey}'))";
                        foreach (Microsoft.Azure.Graphs.Elements.Edge edge in ExecuteCommandQueryEdge(getEdgeQuery))
                        {
                            try
                            {
                                edgeProperties = edge.GetProperties().ToList();
                            }
                            catch (NullReferenceException)
                            {
                                continue;
                            }

                            if (edgeProperties.Any(x => x.Key == propInfo.Name))
                            {
                                edgePropertyValue = edgeProperties.First(x => x.Key == propInfo.Name).Value;

                                if (propInfo.PropertyType.BaseType == typeof(Enum))
                                    propInfo.SetValue(embeddedObject, Convert.ToInt32(edgePropertyValue));
                                else if (propInfo.PropertyType == typeof(DateTime))
                                    propInfo.SetValue(embeddedObject, DateTime.Parse(edgePropertyValue.ToString()));
                                else
                                    propInfo.SetValue(embeddedObject, Convert.ChangeType(edgePropertyValue, propInfo.PropertyType));
                            }
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

        public static object GetGenericRepository(Type genericArgument)
        {
            Type thisType = typeof(GraphRepositoy<T>), currentRepoType = null, interfaceType = null;
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
                            genericRepository = Activator.CreateInstance(currentRepoType);
                        }
                    }
                }
            }

            return genericRepository;
        }

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        private void ExecuteCommandEdge(string CommandString)
        {
            CommandString = CommandString.Replace("$", "\\$");

            IDocumentQuery<Microsoft.Azure.Graphs.Elements.Edge> query = documentClient.CreateGremlinQuery<Microsoft.Azure.Graphs.Elements.Edge>(documentCollection, CommandString);
            var feedResponse = query.ExecuteNextAsync<Microsoft.Azure.Graphs.Elements.Edge>().Result;
        }

        private string ExecuteCommandVertex(string CommandString)
        {
            //using (StreamWriter sw = new StreamWriter(@"C:\apache-tinkerpop-gremlin-console-3.3.3\customData\commandLog.log", true))
            //{
            //    sw.WriteLine(CommandString);
            //}

            CommandString = CommandString.Replace("$", "\\$");

            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString);
            var feedResponse = query.ExecuteNextAsync<Vertex>().Result;
            return feedResponse.First().Id.ToString();
        }

        protected FeedResponse<Vertex> ExecuteCommandQueryVertex(string CommandString)
        {
            CommandString = CommandString.Replace("$", "\\$");

            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString);
            return query.ExecuteNextAsync<Vertex>().Result;
        }

        private FeedResponse<Microsoft.Azure.Graphs.Elements.Edge> ExecuteCommandQueryEdge(string CommandString)
        {
            CommandString = CommandString.Replace("$", "\\$");

            IDocumentQuery<Microsoft.Azure.Graphs.Elements.Edge> query = documentClient.CreateGremlinQuery<Microsoft.Azure.Graphs.Elements.Edge>(documentCollection, CommandString);
            return query.ExecuteNextAsync<Microsoft.Azure.Graphs.Elements.Edge>().Result;
        }

        private FeedResponse<dynamic> ExecuteCommandQueryDynamic(string CommandString)
        {
            CommandString = CommandString.Replace("$", "\\$");

            IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(documentCollection, CommandString);
            return query.ExecuteNextAsync<dynamic>().Result;
        }

        public void CreateEdge(GraphObject from, GraphObject to, Attributes.Edge EdgeAttribute)
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
                        if (prop.PropertyType.BaseType == typeof(Enum))
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

        public bool HasEdge(Attributes.Edge edge, GraphObject from, GraphObject to)
        {
            bool hasEdge;
            string findEdgeQuery = $"g.E().hasLabel('{edge.EdgeName}').has('_vertexId', '{ from.GraphKey }').has('_sink', '{to.GraphKey}').count()";
            FeedResponse<dynamic> reponse = ExecuteCommandQueryDynamic(findEdgeQuery);

            hasEdge = Convert.ToInt32(reponse.First()) > 0;

            return hasEdge;
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
            var a = documentClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("Graph_CV", "CVs")).Result;
            var b = documentClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri("Graph_CV"), new DocumentCollection { Id = "CVs" }).Result;
        }
    }

    public sealed class GraphRepositoy
    {
        public static object GetGenericRepository(Type genericArgument)
        {
            Type thisType = typeof(GraphRepositoy), currentRepoType = null, interfaceType = null;
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
                            genericRepository = Activator.CreateInstance(currentRepoType);
                        }
                    }
                }
            }

            return genericRepository;
        }

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }
    }
}
