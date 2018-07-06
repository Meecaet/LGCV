using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches
{
    public static class GraphConfig
    {
        public static DocumentClient DocumentClient;
        public static DocumentCollection DocumentCollection;

        public static string Endpoint, Primarykey, Database, GraphCollection, ConnectionString;

        public static void SetGraphDataBaseConnection(string endpoint, string primaryKey, string database, string graphCollection)
        {
            Endpoint = endpoint;
            Primarykey = primaryKey;
            Database = database;
            GraphCollection = graphCollection;

            DocumentClient = new DocumentClient(new Uri(Endpoint), Primarykey);
            DocumentCollection = DocumentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Database, GraphCollection), new RequestOptions { OfferThroughput = 400 }).Result;
        }

        public static void SetGraphDataBaseConnection(DocumentClient documentClient, DocumentCollection documentCollection)
        {
            DocumentClient = documentClient;
            DocumentCollection = documentCollection;
        }

        public static void SetGraphDataBaseConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
