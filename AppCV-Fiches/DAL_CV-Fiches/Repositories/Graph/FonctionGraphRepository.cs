using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class FonctionGraphRepository : GraphRepositoy<Fonction>
    {
        public FonctionGraphRepository()
        {
        }

        public FonctionGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public FonctionGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public FonctionGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
