using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using TradeNest.Tests.Common;

namespace TradeNest.Data.Tests.RepositoriesTests;

[TestFixture]
public class UsersRepositoryTests : BaseRepositoriesTest
{
    private UsersRepository _usersRepository;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<RoleManager<ApplicationRole>> _roleManagerMock;

    [SetUp]
    public async Task SetUp()
    {
        this._userManagerMock = new Mock<UserManager<ApplicationUser>>();
        this._roleManagerMock = new Mock<RoleManager<ApplicationRole>>();

        this._usersRepository = new UsersRepository(
            this.DbContext,
            this._userManagerMock.Object,
            this._roleManagerMock.Object);

        await base.SetUp();
    }

    [TearDown]
    public async Task TearDown()
    {
        this._usersRepository.Dispose();
        await base.SetUp();
    }

    [Test]
    public async Task GetAllUsersWithTheirRolesAsync_WhenNoUsersExist_ReturnsEmptyEnumerable()
    {
        // Arrange & Act
        IEnumerable<ApplicationUser> res = await this._usersRepository
            .GetAllUsersWithTheirRolesAsync();

        // Assert
        Assert.IsEmpty(res);
    }

    [Test]
    public async Task GetAllUsersWithTheirRolesAsync_WorksCorrectly()
    {
        // Arrange
        string targetString = RandomStringGenerator.RandomString(length: 10);
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid(), };
        ApplicationRole role = new ApplicationRole() { Id = Guid.NewGuid() };
        ApplicationUserRole userRole = new ApplicationUserRole() { RoleId = role.Id, UserId = user.Id };
        ApplicationUser targetUser = new ApplicationUser() { Id = Guid.NewGuid(), UserName = targetString };
        ApplicationRole targetRole = new ApplicationRole() { Id = Guid.NewGuid() };
        ApplicationUserRole targetUserRole = new ApplicationUserRole() { RoleId = targetRole.Id, UserId = targetUser.Id };
    
        await this.SeedAsync(user);
        await this.SeedAsync(role);
        await this.SeedAsync(userRole);
        await this.SeedAsync(targetUser);
        await this.SeedAsync(targetRole);
        await this.SeedAsync(targetUserRole);

        // Act
        IEnumerable<ApplicationUser> res = await this._usersRepository
            .GetAllUsersWithTheirRolesAsync(a => a.UserName == targetString);

        // Assert
        Assert.That(res.Count, Is.EqualTo(1));
        Assert.That(res.First().Id, Is.EqualTo(targetUser.Id));
        Assert.That(res.First().UserRoles.First().RoleId, Is.EqualTo(targetRole.Id));
    }

    [Test]
    public async Task GetAllRolesAsync_WhenNoRoles_ReturnsEmptyEnumerable()
    {
        // Arrange & Act
        IEnumerable<ApplicationRole> res = await this._usersRepository.GetAllRolesAsync();

        // Assert
        Assert.IsEmpty(res);
    }

    [Test]
    public async Task GetAllRolesAsync_WorksCorrectly()
    {
        // Arrange
        ApplicationRole role = new ApplicationRole() { Id = Guid.NewGuid() };
        await this.SeedAsync(role);

        // Act
        IEnumerable<ApplicationRole> res = await this._usersRepository
            .GetAllRolesAsync();

        // Assert
        Assert.That(res.First().Id, Is.EqualTo(role.Id));       
    }

    [Test]
    public async Task ExistsByIdWithForgottenIncludedAsync_WhenDoesntExist_ReturnsFalse()
    {
        // Arrange & Act
        bool res = await this._usersRepository
            .ExistsByIdWithForgottenIncludedAsync(It.IsAny<Guid>());
        
        // Assert
        Assert.That(res, Is.False);
    }

    [Test]
    public async Task ExistsByIdWithForgottenIncludedAsync_WorksCorrectly()
    {
        // Arrange
        Guid targetGuid = Guid.NewGuid();
        ApplicationUser user = new ApplicationUser() { Id = targetGuid, PersonalInformationIsDeleted = true };
        await this.SeedAsync(user);

        // Act
        bool res = await this._usersRepository
            .ExistsByIdWithForgottenIncludedAsync(targetGuid);

        // Assert
        Assert.That(res, Is.True);
    }

    [Test]
    public async Task FindByIdWithForgottenIncludedAsync_WhenDoesntExist_ReturnsNull()
    {
        // Arrange & Act
        ApplicationUser? nonExistent = await this._usersRepository
            .FindByIdWithForgottenIncludedAsync(It.IsAny<Guid>());

        // Assert
        Assert.IsNull(nonExistent);
    }

    [Test]
    public async Task FindByIdWithForgottenIncludedAsync_WorksCorrectly()
    {
        // Arrange
        Guid targetGuid = Guid.NewGuid();
        ApplicationUser user = new ApplicationUser() { Id = targetGuid, PersonalInformationIsDeleted = true };
        await this.SeedAsync(user);

        // Act
        ApplicationUser? res = await this._usersRepository
            .FindByIdWithForgottenIncludedAsync(targetGuid);

        // Assert
        Assert.NotNull(res);
        Assert.That(res.Id, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task IsUserAdminUserByIdAsync_IfUserNotAdmin_ReturnsFalse()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid() };
        await this.SeedAsync(user);
        
        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        bool res = await this._usersRepository.IsUserAdminUserByIdAsync(user.Id);

        // Assert
        Assert.That(res, Is.False);
    }

    [Test]
    public async Task IsUserAdminUserByIdAsync_IfUserDoesNotExist_ReturnsFalse()
    {
        // Arrange & Act
        bool res = await this._usersRepository.IsUserAdminUserByIdAsync(It.IsAny<Guid>());

        // Assert
        Assert.That(res, Is.False);
    }

    [Test]
    public async Task IsUserAdminUserByIdAsync_IfUserIsAdmin_ReturnsTrue()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid(), };
        await this.SeedAsync(user);

        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        bool res = await this._usersRepository.IsUserAdminUserByIdAsync(user.Id);

        // Assert
        Assert.That(res, Is.True);
    }

    [Test]
    public async Task IsUserAdminUserAsync_IfUserNotAdmin_ReturnsFalse()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid(), };
        await this.SeedAsync(user);

        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        // Act
        bool res = await this._usersRepository.IsUserAdminUserAsync(user);

        // Assert
        Assert.That(res, Is.False);
    }

    [Test]
    public async Task IsUserAdminUserAsync_IfUserIsAdmin_ReturnsTrue()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid(), };
        await this.SeedAsync(user);

        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
            
        // Act
        bool res = await this._usersRepository.IsUserAdminUserAsync(user);

        // Assert
        Assert.That(res, Is.True);
    }

    [Test]
    public async Task AddAsync_WorksCorrectly()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid() };
        await this.SeedAsync(user);

        this._userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        bool res = await this._usersRepository
            .AddAsync(user, RandomStringGenerator.RandomString(length: 10));

        // Assert
        Assert.That(res, Is.True);
    }

    [Test]
    public async Task AddAsync_WhenFails_ReturnsFalse()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid() };
        await this.SeedAsync(user);

        this._userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        bool res = await this._usersRepository
            .AddAsync(user, RandomStringGenerator.RandomString(length: 10));

        // Assert
        Assert.That(res, Is.False);
    }
}
