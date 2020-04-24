using System.Collections.Generic;
using System.Linq;
using BackOfficeBase.Utilities.Collections.Extensions;
using Xunit;

namespace BackOfficeBase.Tests.Utilities.Extensions
{
    public class EnumerableExtensionsTests : UtilitiesTestBase
    {
        private readonly List<string> _testList = new List<string>();

        public EnumerableExtensionsTests()
        {
            for (var i = 0; i < 23; i++)
            {
                _testList.Add(i.ToString());
            }
        }

        [Fact]
        public void Should_Paged_By()
        {
            var pagedList = _testList.PagedBy(0, 10);

            Assert.Equal(10, pagedList.Count());
        }

        [Fact]
        public void Should_To_Paged_List_Result()
        {
            var pagedList = _testList.ToPagedListResult(_testList.Count);

            Assert.Equal(_testList.Count, pagedList.TotalCount);
            Assert.Equal(_testList.Count, pagedList.Items.Count());
        }
    }
}
