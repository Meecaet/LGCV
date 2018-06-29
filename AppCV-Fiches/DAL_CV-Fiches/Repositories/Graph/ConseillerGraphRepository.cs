using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class ConseillerGraphRepository : GraphRepositoy<Conseiller>
    {
        public ConseillerGraphRepository()
        {
        }

        public ConseillerGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public ConseillerGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public ConseillerGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
