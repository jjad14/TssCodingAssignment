using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Data;
using TssCodingAssignment.DataAccess.Repository.IRepository;
using TssCodingAssignment.Models;

namespace TssCodingAssignment.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
           _db = db;
        }

        public void Update(Category category)
        {
            var objectFromDb = _db.Categories.FirstOrDefault(c => c.Id == category.Id);

            if (objectFromDb != null) 
            {
                objectFromDb.Name = category.Name;

                _db.SaveChanges();            
            }

        }
    }
}
