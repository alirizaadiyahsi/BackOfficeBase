using BackOfficeBase.DataAccess;

namespace BackOfficeBase.Tests.IntegrationTests.DataBuilder
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
            new TestUsersDataBuilder(_dbContext).SeedData();
        }
    }
}
