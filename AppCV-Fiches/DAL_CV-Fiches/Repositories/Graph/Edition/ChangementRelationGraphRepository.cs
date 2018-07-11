using DAL_CV_Fiches.Models.Graph;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public class ChangementRelationGraphRepository : GraphRepositoy<ChangementRelation>
    {
        public ChangementRelationGraphRepository()
        {
        }

        public ChangementRelationGraphRepository(DocumentClient documentClient, DocumentCollection documentCollection) : base(documentClient, documentCollection)
        {
        }

        public ChangementRelationGraphRepository(string Database, string Graph) : base(Database, Graph)
        {
        }

        public ChangementRelationGraphRepository(string Endpoint, string Key, string Database, string Graph) : base(Endpoint, Key, Database, Graph)
        {
        }
    }
}
