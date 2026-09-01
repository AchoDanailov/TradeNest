using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Moq;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using static TradeNest.Tests.Common.RandomStringGenerator;

namespace TradeNest.Data.IntegrationTests.RepositoriesTests;

public class AdminsRepositoryTests : IntegrationTestsBase
{
    private AdminsRepository _repository;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;

    [SetUp]
    public void SetUp()
    {
        Mock<IUserStore<ApplicationUser>> store = new Mock<IUserStore<ApplicationUser>>();
        this._userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        this._repository = new AdminsRepository(DbContext, _userManagerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        this._repository.Dispose();
    }

    [Test]
    public async Task IsUserAdminByUserIdAsync_ShouldReturnTrue_WhenUserIsAdmin()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser
        {
            UserName = RandomString(length: 10),
            Email = $"{RandomString(length: 10)}@test.com"
        };
        Admin admin = new Admin { User = user };
        await SeedAsync(user);
        await SeedAsync(admin);

        this._userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Admin"))
            .ReturnsAsync(true);

        // Act
        bool result = await _repository.IsUserAdminByUserIdAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Test]
    public async Task IsUserAdminByUserIdAsync_ShouldReturnFalse_WhenUserIsNotAdmin()
    {
        // Act
        bool result = await _repository.IsUserAdminByUserIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(result);
    }
}