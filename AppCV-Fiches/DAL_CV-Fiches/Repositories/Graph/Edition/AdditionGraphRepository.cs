using System;
using System.Collections.Generic;
using System.Text;
using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class AdditionGraphRepository : GraphRepositoy<Addition>

    {
        public AdditionGraphRepository()
        {
        }

        public AdditionGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public AdditionGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public AdditionGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
