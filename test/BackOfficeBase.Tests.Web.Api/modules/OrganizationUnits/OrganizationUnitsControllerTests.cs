using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BackOfficeBase.Application.Dto;
using BackOfficeBase.Application.OrganizationUnits;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Modules.Core.Controllers;
using BackOfficeBase.Utilities.Collections;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Web.Api.modules.OrganizationUnits
{
    public class OrganizationUnitsControllerTests : WebApiTestBase
    {
        [Fact]
        public async Task Should_Get_OrganizationUnit_Async()
        {
            var organizationUnitAppServiceMock = new Mock<IOrganizationUnitAppService>();
            organizationUnitAppServiceMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new OrganizationUnitOutput
            {
                Id = Guid.NewGuid(),
                SelectedUsers = new[] { GetTestUserOutput("test_user_for_get_ou") },
                SelectedRoles = new[] { GetTestRoleOutput("test_role_for_get_ou") }
            });

            var organizationUnitsController = new OrganizationUnitsController(organizationUnitAppServiceMock.Object);
            var actionResult = await organizationUnitsController.GetOrganizationUnits(Guid.NewGuid());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var organizationUnitOutput = Assert.IsType<OrganizationUnitOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(organizationUnitOutput.SelectedUsers.Any());
            Assert.True(organizationUnitOutput.SelectedRoles.Any());
        }

        [Fact]
        public async Task Should_Get_OrganizationUnit_List_Async()
        {
            var organizationUnitAppServiceMock = new Mock<IOrganizationUnitAppService>();
            organizationUnitAppServiceMock.Setup(x => x.GetListAsync(It.IsAny<PagedListInput>())).ReturnsAsync(new PagedListResult<OrganizationUnitListOutput>
            {
                Items = new List<OrganizationUnitListOutput>
                {
                    new OrganizationUnitListOutput {Name = "test_organizationUnit_1", Id = Guid.NewGuid()},
                    new OrganizationUnitListOutput {Name = "test_organizationUnit_2", Id = Guid.NewGuid()},
                    new OrganizationUnitListOutput {Name = "test_organizationUnit_3", Id = Guid.NewGuid()},
                    new OrganizationUnitListOutput {Name = "test_organizationUnit_4", Id = Guid.NewGuid()},
                    new OrganizationUnitListOutput {Name = "test_organizationUnit_5", Id = Guid.NewGuid()}
                },
                TotalCount = 10
            });

            var organizationUnitsController = new OrganizationUnitsController(organizationUnitAppServiceMock.Object);
            var actionResult = await organizationUnitsController.GetOrganizationUnits(new PagedListInput());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var organizationUnitPagedListResult = Assert.IsType<PagedListResult<OrganizationUnitListOutput>>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.Equal(10, organizationUnitPagedListResult.TotalCount);
            Assert.Equal(5, organizationUnitPagedListResult.Items.Count());
        }

        [Fact]
        public async Task Should_Create_OrganizationUnit_Async()
        {
            var organizationUnitAppServiceMock = new Mock<IOrganizationUnitAppService>();
            organizationUnitAppServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateOrganizationUnitInput>())).ReturnsAsync(new OrganizationUnitOutput
            {
                Id = Guid.NewGuid(),
                SelectedUsers = new[] { GetTestUserOutput("test_user_for_insert_ou") },
                SelectedRoles = new[] { GetTestRoleOutput("test_role_for_insert_ou") }
            });

            var organizationUnitsController = new OrganizationUnitsController(organizationUnitAppServiceMock.Object);
            var actionResult = await organizationUnitsController.PostOrganizationUnits(new CreateOrganizationUnitInput());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var organizationUnitOutput = Assert.IsType<OrganizationUnitOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(organizationUnitOutput.SelectedUsers.Any());
            Assert.True(organizationUnitOutput.SelectedRoles.Any());
        }

        [Fact]
        public async Task Should_Update_OrganizationUnit()
        {
            var organizationUnitAppServiceMock = new Mock<IOrganizationUnitAppService>();
            organizationUnitAppServiceMock.Setup(x => x.Update(It.IsAny<UpdateOrganizationUnitInput>())).Returns(new OrganizationUnitOutput
            {
                Id = Guid.NewGuid(),
                SelectedUsers = new[] { GetTestUserOutput("test_user_for_update_ou") },
                SelectedRoles = new[] { GetTestRoleOutput("test_role_for_update_ou") }
            });

            var organizationUnitsController = new OrganizationUnitsController(organizationUnitAppServiceMock.Object);
            var actionResult = await organizationUnitsController.PutOrganizationUnits(new UpdateOrganizationUnitInput());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var organizationUnitOutput = Assert.IsType<OrganizationUnitOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(organizationUnitOutput.SelectedUsers.Any());
            Assert.True(organizationUnitOutput.SelectedRoles.Any());
        }

        [Fact]
        public async Task Should_Delete_OrganizationUnit_Async()
        {
            var organizationUnitAppServiceMock = new Mock<IOrganizationUnitAppService>();
            organizationUnitAppServiceMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(new OrganizationUnitOutput
            {
                Id = Guid.NewGuid(),
                SelectedUsers = new[] { GetTestUserOutput("test_user_for_delete_ou") },
                SelectedRoles = new[] { GetTestRoleOutput("test_role_for_delete_ou") }
            });

            var organizationUnitsController = new OrganizationUnitsController(organizationUnitAppServiceMock.Object);
            var actionResult = await organizationUnitsController.DeleteOrganizationUnits(Guid.NewGuid());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var organizationUnitOutput = Assert.IsType<OrganizationUnitOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(organizationUnitOutput.SelectedUsers.Any());
            Assert.True(organizationUnitOutput.SelectedRoles.Any());
        }
    }
}
