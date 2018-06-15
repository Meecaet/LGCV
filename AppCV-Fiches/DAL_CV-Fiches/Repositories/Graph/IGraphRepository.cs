using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public interface IGraphRepository<T>
    {
        void Add(T obj);
        void Update(T obj);
        void Delete(T obj);
        T GetOne(string id);
        List<T> GetAll();
        List<T> Search(T searchObject);
    }
}
