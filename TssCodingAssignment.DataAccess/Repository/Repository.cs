using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Data;
using TssCodingAssignment.DataAccess.Repository.IRepository;

namespace TssCodingAssignment.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        // add entity
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        // get entity by id
        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        // get all entities that optionally have a filter, order and includeProperties (eager loading)
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // for filtering
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // for eager loading
            if (includeProperties != null) 
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) 
                {
                    query = query.Include(includeProp);
                }
            }

            // if orderby is specified
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            // return a list of records
            return query.ToList();
        }

        // get entity that matches parameters
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // if there are filters
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // if there are includes
            if (includeProperties != null)
            {
                // add each include property individually
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // return a single record
            return query.FirstOrDefault();
        }

        // remove entity by id
        public void Remove(int id) { 
            T entity = dbSet.Find(id);
            Remove(entity);
        }

        // remove entity
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        // remove entities
        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

    }
}
