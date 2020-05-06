using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Roles;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Identity;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Modules.Authorization.Controllers;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Web.Api.modules.Authorization
{
    public class RolesControllerTests : WebApiTestBase
    {
        [Fact]
        public async Task Should_Get_Role_Async()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new RoleOutput
            {
                Name = "test_role",
                Id = Guid.NewGuid()
            });

            var rolesController = new RolesController(roleAppServiceMock.Object, new Mock<IIdentityAppService>().Object);
            var actionResult = await rolesController.GetRoles(Guid.NewGuid());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var roleOutput = Assert.IsType<RoleOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.Equal("test_role", roleOutput.Name);
        }

        [Fact]
        public async Task Should_Get_Role_List_Async()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.GetListAsync(It.IsAny<PagedListInput>())).ReturnsAsync(new PagedListResult<RoleListOutput>
            {
                Items = new List<RoleListOutput>
                {
                    new RoleListOutput {Name = "test_role_1", Id = Guid.NewGuid()},
                    new RoleListOutput {Name = "test_role_2", Id = Guid.NewGuid()},
                    new RoleListOutput {Name = "test_role_3", Id = Guid.NewGuid()},
                    new RoleListOutput {Name = "test_role_4", Id = Guid.NewGuid()},
                    new RoleListOutput {Name = "test_role_5", Id = Guid.NewGuid()}
                },
                TotalCount = 10
            });

            var rolesController = new RolesController(roleAppServiceMock.Object, new Mock<IIdentityAppService>().Object);
            var actionResult = await rolesController.GetRoles(new PagedListInput());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var rolePagedListResult = Assert.IsType<PagedListResult<RoleListOutput>>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.Equal(10, rolePagedListResult.TotalCount);
            Assert.Equal(5, rolePagedListResult.Items.Count());
        }

        [Fact]
        public async Task Should_Create_Role_Async()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateRoleInput>())).ReturnsAsync(new RoleOutput
            {
                Name = "test_role",
                Id = Guid.NewGuid()
            });

            var rolesController = new RolesController(roleAppServiceMock.Object, new Mock<IIdentityAppService>().Object);
            var actionResult = await rolesController.PostRoles(new CreateRoleInput());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var roleOutput = Assert.IsType<RoleOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.Equal("test_role", roleOutput.Name);
        }

        [Fact]
        public async Task Should_Not_Create_Role_Async()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateRoleInput>())).ReturnsAsync(new RoleOutput
            {
                Name = "test_role",
                Id = Guid.NewGuid()
            });

            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindRoleByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new RoleOutput
                {
                    Id = Guid.NewGuid(),
                    Name = "test_role_" + Guid.NewGuid()
                });

            var rolesController = new RolesController(roleAppServiceMock.Object, identityAppServiceMock.Object);
            var actionResult = await rolesController.PostRoles(new CreateRoleInput());

            var conflictObjectResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
            var message = Assert.IsType<string>(conflictObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.Conflict, conflictObjectResult.StatusCode);
            Assert.Equal(UserFriendlyMessages.RoleNameAlreadyExist, message);
        }

        [Fact]
        public async Task Should_Update_Role()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.Update(It.IsAny<UpdateRoleInput>())).Returns(new RoleOutput
            {
                Name = "test_role",
                Id = Guid.NewGuid()
            });

            var rolesController = new RolesController(roleAppServiceMock.Object, new Mock<IIdentityAppService>().Object);
            var actionResult = await rolesController.PutRoles(new UpdateRoleInput());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var roleOutput = Assert.IsType<RoleOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.Equal("test_role", roleOutput.Name);
        }

        [Fact]
        public async Task Should_Not_Update_Role()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.Update(It.IsAny<UpdateRoleInput>())).Returns(new RoleOutput
            {
                Name = "test_role",
                Id = Guid.NewGuid()
            });

            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindRoleByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new RoleOutput
                {
                    Id = Guid.NewGuid(),
                    Name = "test_role_" + Guid.NewGuid()
                });

            var rolesController = new RolesController(roleAppServiceMock.Object, identityAppServiceMock.Object);
            var actionResult = await rolesController.PutRoles(new UpdateRoleInput());

            var conflictObjectResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
            var message = Assert.IsType<string>(conflictObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.Conflict, conflictObjectResult.StatusCode);
            Assert.Equal(UserFriendlyMessages.RoleNameAlreadyExist, message);
        }

        [Fact]
        public async Task Should_Delete_Role_Async()
        {
            var roleAppServiceMock = new Mock<IRoleAppService>();
            roleAppServiceMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(new RoleOutput
            {
                Name = "test_role",
                Id = Guid.NewGuid()
            });

            var rolesController = new RolesController(roleAppServiceMock.Object, new Mock<IIdentityAppService>().Object);
            var actionResult = await rolesController.DeleteRoles(Guid.NewGuid());

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var roleOutput = Assert.IsType<RoleOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.Equal("test_role", roleOutput.Name);
        }
    }
}
