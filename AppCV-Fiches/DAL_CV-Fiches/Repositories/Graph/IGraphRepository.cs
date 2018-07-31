using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public interface IGraphRepository<T>
    {
        void Add(T obj, bool cascade = true);
        void Update(T obj, GraphObject parent);
        void Delete(T obj);
        T GetOne(string id);
        List<T> GetAll();
        List<T> Search(T searchObject);
        List<T> Search(T searchObject, bool returnDefault);
        List<T> Search(Dictionary<string, object> properties);
        List<T> Search(Dictionary<string, object> properties, bool returnDefault);
    }
}
