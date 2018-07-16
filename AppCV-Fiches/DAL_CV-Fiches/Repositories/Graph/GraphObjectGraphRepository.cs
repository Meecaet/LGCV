using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    class GraphObjectGraphRepository : GraphRepositoy<GraphObject>
    {
        public GraphObjectGraphRepository()
        {
        }

        public GraphObjectGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public GraphObjectGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public GraphObjectGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
