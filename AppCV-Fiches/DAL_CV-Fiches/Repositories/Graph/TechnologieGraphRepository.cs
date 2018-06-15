using DAL_CV_Fiches.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class TechnologieGraphRepository : GraphRepositoy<Technologie>
    {
        public TechnologieGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public TechnologieGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public TechnologieGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
