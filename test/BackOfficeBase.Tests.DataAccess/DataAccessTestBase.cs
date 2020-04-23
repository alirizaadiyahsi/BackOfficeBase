using BackOfficeBase.Tests.Shared;
using BackOfficeBase.Tests.Shared.DataAccess;

namespace BackOfficeBase.Tests.DataAccess
{
    public class DataAccessTestBase : TestBase
    {
        protected readonly BackOfficeBaseDbContextTest DbContextTest;

        public DataAccessTestBase()
        {
            DbContextTest = GetDbContextTest();
        }
    }
}
