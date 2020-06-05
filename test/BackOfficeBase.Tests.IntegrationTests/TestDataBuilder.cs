using BackOfficeBase.DataAccess;
using BackOfficeBase.Tests.IntegrationTests.AuthenticationTests.DataBuilder;

namespace BackOfficeBase.Tests.IntegrationTests
{
    public class TestDataBuilder
    {
        private readonly BackOfficeBaseDbContext _dbContext;

        public TestDataBuilder(BackOfficeBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            new TestDataBuilderForAccount(_dbContext).SeedData();
        }
    }
}
