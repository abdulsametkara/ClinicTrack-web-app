using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickDataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DatabaseBaglanti _dbBaglanti;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(DatabaseBaglanti dbBaglanti)
        {
            _dbBaglanti = dbBaglanti;
            _dbSet = _dbBaglanti.Set<T>();
        }
        public void Create(T entity)
        {
            _dbSet.Add(entity);
            _dbBaglanti.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _dbBaglanti.SaveChanges();
        }

        public void DeleteByRange(List<T> entities)
        {
            _dbSet.RemoveRange(entities);
            _dbBaglanti.SaveChanges();
        }

        public IQueryable<T> GetAll()
        {
           return _dbSet.AsQueryable();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);

        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _dbBaglanti.SaveChanges();
        }
    }
}
