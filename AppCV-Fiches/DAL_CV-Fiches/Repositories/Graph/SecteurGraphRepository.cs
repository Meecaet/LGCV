using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class SecteurGraphRepository : GraphRepositoy<Secteur>
    {
        public SecteurGraphRepository()
        {
        }

        public SecteurGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public SecteurGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public SecteurGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
