using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class CVGraphRepository : GraphRepositoy<CV>
    {
        public CVGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public CVGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public CVGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }

        public void DeleteAllDocs()
        {
            var a = documentClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("Graphe_Essay", "graph_cv")).Result;
            var b = documentClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri("Graphe_Essay"), new DocumentCollection { Id = "graph_cv" }).Result;
        }
    }
}
