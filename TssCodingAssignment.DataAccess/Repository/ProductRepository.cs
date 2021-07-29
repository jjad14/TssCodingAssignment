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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objectFromDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);

            if (objectFromDb != null) 
            {
                // user might not upload a new image
                if (product.ImageUrl != null)
                {
                    objectFromDb.ImageUrl = product.ImageUrl;
                }

                // map the new product to existing product
                objectFromDb.Name = product.Name;
                objectFromDb.Description = product.Description;
                objectFromDb.Price = product.Price;
                objectFromDb.CategoryId = product.CategoryId;

            }
        }
    }
}
