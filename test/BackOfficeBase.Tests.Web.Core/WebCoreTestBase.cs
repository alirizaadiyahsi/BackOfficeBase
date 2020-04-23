using BackOfficeBase.Tests.Shared;
using BackOfficeBase.Tests.Shared.DataAccess;

namespace BackOfficeBase.Tests.Web.Core
{
    public class WebCoreTestBase : TestBase
    {
        protected readonly TestBackOfficeBaseDbContext DbContextTest;

        public WebCoreTestBase()
        {
            DbContextTest = GetDbContextTest();
        }
    }
}
