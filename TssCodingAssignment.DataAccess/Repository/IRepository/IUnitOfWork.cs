using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TssCodingAssignment.DataAccess.Repository.IRepository
{
    // UoW class coordinates the work of multiple repositories by creating a single database context shared by all of them
    // Controller -> UoW (repos and context) -> EF Core and Database
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }

        void Save();
    }
}