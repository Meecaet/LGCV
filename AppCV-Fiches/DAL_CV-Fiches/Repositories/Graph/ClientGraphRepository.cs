using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class ClientGraphRepository : GraphRepositoy<Client>
    {
        public ClientGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public ClientGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public ClientGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
