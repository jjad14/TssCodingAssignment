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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db)
        : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart cart)
        {
            _db.Update(cart);
        }
    }
}
