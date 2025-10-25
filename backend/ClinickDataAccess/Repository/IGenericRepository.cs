using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickDataAccess.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        #region CRUD
        void Create(T entity);
        IQueryable<T> GetAll();
        T GetById(int id);
        void Update(T entity);
        void Delete(T entity);
        void DeleteByRange(List<T> entities);
        #endregion
    }
}
