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
                    object genericRepository = GetGenericRepository(tuple.vertexType);

                    for (int i = 0; i < tuple.ListOfVertexes.Count; i++)
                    {
                        item = (GraphObject)tuple.ListOfVertexes[i];
                        if(string.IsNullOrEmpty(item.graphKey))
                            genericRepository.GetType().GetMethod("Add").Invoke(genericRepository, new object[] { item });

                        CreateEdge(obj, item, tuple.EdgeAttribute);
                    }
                }

                AddHasEdges = false;
                posponedCreateEdge.Clear();
            }
        }

        public void Delete<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            List<T> listOfObj = new List<T>();
            foreach (Vertex vertex in ExecuteCommandQuery(GetGetAllQuery()))
            {
                listOfObj.Add(GetObjectFromVertex(vertex));
            }

            return listOfObj;
        }

        public void GetOne<T>(int id)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T obj)
        {
            throw new NotImplementedException();
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
                        query += $".property('{item.Name}', '{value}')";
                    else if (value is Int32)
                        query += $".property('{item.Name}', {value})";
                    else if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Attributes.Edge att = (Attributes.Edge)item.GetCustomAttribute(typeof(Attributes.Edge));
                        if (att != null)
                        {
                            posponedCreateEdge = new List<PostponedEdgeCreator>();
                            posponedCreateEdge.Add(new PostponedEdgeCreator { EdgeAttribute = att, ListOfVertexes = (IList)value, vertexType = item.PropertyType.GetGenericArguments()[0] });
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

        protected string GetGetAllQuery()
        {
            Type thisType = typeof(T);
            return $"g.V().hasLabel('{thisType.Name}')";
        }

        protected T GetObjectFromVertex (Vertex vertex)
        {
            Type thisType = typeof(T);
            T genObj = new T();

            foreach (var item in vertex.GetVertexProperties())
            {
                PropertyInfo prop = thisType.GetProperty(item.Key);

                if(prop != null)
                    prop.SetValue(genObj, item.Value);
            }

            return genObj;
        }

        private string ExecuteCommand(string CommandString)
        {
            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString);
            var feedResponse = query.ExecuteNextAsync<Vertex>().Result;
            return feedResponse.First().Id.ToString();
        }

        private FeedResponse<Vertex> ExecuteCommandQuery(string CommandString)
        {
            IDocumentQuery<Vertex> query = documentClient.CreateGremlinQuery<Vertex>(documentCollection, CommandString);
            return query.ExecuteNextAsync<Vertex>().Result;
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
        internal Type vertexType { get; set; }
        internal Attributes.Edge EdgeAttribute { get; set; }
        internal IList ListOfVertexes { get; set; }
    }
}
