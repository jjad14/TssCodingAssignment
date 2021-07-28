using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TssCodingAssignment.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        // get entity by id
        T Get(int id);

        // get all entities by filter?, order?, includeProperties
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
        );

        // get first entity by filter?, includeProperties
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
        );

        // add entity
        void Add(T entity);

        // remove entity by id
        void Remove(int id);

        // remove entity using full entitiy
        void Remove(T entity);

        // remove a range of entities 
        void RemoveRange(IEnumerable<T> entities);
    }
}
