using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    class CertificationGraphRepository : GraphRepositoy<Certification>
    {
        public CertificationGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public CertificationGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public CertificationGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
