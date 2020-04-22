using BackOfficeBase.DataAccess;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Tests.Shared.DataAccess
{
    public class BackOfficeBaseDbContextTest : BackOfficeBaseDbContext
    {
        public BackOfficeBaseDbContextTest(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}