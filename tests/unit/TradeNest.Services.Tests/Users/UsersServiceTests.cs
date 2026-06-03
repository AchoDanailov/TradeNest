using Moq;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;

namespace TradeNest.Services.Tests.Users;

[TestFixture]
public class UsersServiceTests
{
    private Mock<IUsersRepository> _usersRepositoryMock;
    private Mock<IAdminsRepository> _adminsRepositoryMock;
    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IUsersMapper> _usersMapperMock;
    private UsersService _usersService;

    [SetUp]
    public void SetUp()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _adminsRepositoryMock = new Mock<IAdminsRepository>();
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _usersMapperMock = new Mock<IUsersMapper>();

        _usersService = new UsersService(
            _usersRepositoryMock.Object,
            _adminsRepositoryMock.Object,
            _productsRepositoryMock.Object,
            _usersMapperMock.Object);
    }

    [Test]
    public void GetAllUsersAsync_ShouldThrowUnauthorizedOperationException_WhenUserIsNotAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedOperationException>(async () => 
            await _usersService.GetAllUsersAsync(userId));
    }

    [Test]
    public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUserIsAdmin()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var users = new List<ApplicationUser> { new ApplicationUser { Id = Guid.NewGuid() } };
        var userDtos = new List<UserDto> { new UserDto { Id = users[0].Id } };

        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(adminId))
            .ReturnsAsync(true);

        _usersRepositoryMock
            .Setup(r => r.GetAllUsersWithTheirRolesAsync(null, true))
            .ReturnsAsync(users);

        _usersMapperMock
            .Setup(m => m.ToUserDtos(users))
            .Returns(userDtos);

        // Act
        var result = await _usersService.GetAllUsersAsync(adminId);

        // Assert
        Assert.That(result, Is.EqualTo(userDtos));
    }

    [Test]
    public void DeleteUserByIdAsync_ShouldThrowUnauthorizedOperationException_WhenAdminIsNotAdmin()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(adminId))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedOperationException>(async () => 
            await _usersService.DeleteUserByIdAsync(adminId, Guid.NewGuid()));
    }

    [Test]
    public async Task DeleteUserByIdAsync_ShouldCallDeleteOnRepository_WhenSuccessful()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var userToDeleteId = Guid.NewGuid();
        var userToDelete = new ApplicationUser { Id = userToDeleteId };

        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(adminId))
            .ReturnsAsync(true);

        _usersRepositoryMock
            .Setup(r => r.FindByIdAsync(userToDeleteId))
            .ReturnsAsync(userToDelete);

        // Act
        await _usersService.DeleteUserByIdAsync(adminId, userToDeleteId);

        // Assert
        _usersRepositoryMock.Verify(r => r.DeleteAsync(userToDelete), Times.Once);
    }

    [Test]
    public void ModifyUserRolesAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _usersService.ModifyUserRolesAsync(Guid.NewGuid(), new ModifyUserRolesDto { Id = Guid.Empty }));
    }

    [Test]
    public async Task ModifyUserRolesAsync_ShouldAssignRoles_WhenRequested()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleName = "Seller";
        var user = new ApplicationUser { Id = userId, UserRoles = new List<ApplicationUserRole>() };
        var allRoles = new List<ApplicationRole> { new ApplicationRole { Id = roleId, Name = roleName } };
        
        var modifyDto = new ModifyUserRolesDto 
        { 
            Id = userId, 
            AllRoles = new List<ModifyRoleDto> 
            { 
                new ModifyRoleDto { Id = roleId, RoleName = roleName, IsAssigned = false, IsActionTaken = true } 
            } 
        };

        _adminsRepositoryMock.Setup(r => r.IsUserAdminByUserIdAsync(adminId)).ReturnsAsync(true);
        _usersRepositoryMock.Setup(r => r.GetAllUsersWithTheirRolesAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>(), false))
            .ReturnsAsync(new List<ApplicationUser> { user });
        _usersRepositoryMock.Setup(r => r.GetAllRolesAsync(true)).ReturnsAsync(allRoles);
        _usersRepositoryMock.Setup(r => r.AssignRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);

        // Act
        await _usersService.ModifyUserRolesAsync(adminId, modifyDto);

        // Assert
        _usersRepositoryMock.Verify(r => r.AssignRolesAsync(user, It.Is<IEnumerable<string>>(roles => roles.Contains(roleName))), Times.Once);
    }

    [Test]
    public async Task ModifyUserRolesAsync_ShouldRemoveRoles_WhenRequested()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleName = "Seller";
        var user = new ApplicationUser 
        { 
            Id = userId, 
            UserRoles = new List<ApplicationUserRole> { new ApplicationUserRole { RoleId = roleId } } 
        };
        var allRoles = new List<ApplicationRole> { new ApplicationRole { Id = roleId, Name = roleName } };
        
        var modifyDto = new ModifyUserRolesDto 
        { 
            Id = userId, 
            AllRoles = new List<ModifyRoleDto> 
            { 
                new ModifyRoleDto { Id = roleId, RoleName = roleName, IsAssigned = true, IsActionTaken = true } 
            } 
        };

        _adminsRepositoryMock.Setup(r => r.IsUserAdminByUserIdAsync(adminId)).ReturnsAsync(true);
        _usersRepositoryMock.Setup(r => r.GetAllUsersWithTheirRolesAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>(), false))
            .ReturnsAsync(new List<ApplicationUser> { user });
        _usersRepositoryMock.Setup(r => r.GetAllRolesAsync(true)).ReturnsAsync(allRoles);
        _usersRepositoryMock.Setup(r => r.RemoveUserFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);

        // Act
        await _usersService.ModifyUserRolesAsync(adminId, modifyDto);

        // Assert
        _usersRepositoryMock.Verify(r => r.RemoveUserFromRolesAsync(user, It.Is<IEnumerable<string>>(roles => roles.Contains(roleName))), Times.Once);
    }

    [Test]
    public void RemoveRoleAsync_ShouldThrowInvalidOperationException_WhenTryingToDeleteAdminRole()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var role = new ApplicationRole { Id = roleId, Name = "Admin" };

        _adminsRepositoryMock.Setup(r => r.IsUserAdminByUserIdAsync(adminId)).ReturnsAsync(true);
        _usersRepositoryMock.Setup(r => r.FindRoleByIdAsync(roleId)).ReturnsAsync(role);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _usersService.RemoveRoleAsync(adminId, roleId));
    }
}