using BackOfficeBase.DataAccess;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Tests.Shared.DataAccess
{
    public class TestBackOfficeBaseDbContext : BackOfficeBaseDbContext
    {
        public TestBackOfficeBaseDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}