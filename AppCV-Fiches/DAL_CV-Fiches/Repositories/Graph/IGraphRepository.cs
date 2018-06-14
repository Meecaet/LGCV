using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_CV_Fiches.Repositories.Graph
{
    public interface IGraphRepository<T>
    {
        void Add(T obj);
        void Update<T>(T obj);
        void Delete<T>(T obj);
        void GetOne<T>(int id);
        List<T> GetAll();
    }
}
