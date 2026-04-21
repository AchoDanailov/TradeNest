using Moq;
using Microsoft.AspNetCore.Identity;
using TradeNest.Data.Models;
using TradeNest.Data.Repository;

namespace TradeNest.Data.Tests.Repositories;

[TestFixture]
public class AdminsRepositoryTests : RepositoryTestsBase
{
    private AdminsRepository _repository = null!;
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;

    [SetUp]
    public void SetUp()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _repository = new AdminsRepository(DbContext, _userManagerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
    }

    [Test]
    public async Task IsUserAdminByUserIdAsync_ShouldReturnTrue_WhenUserIsAdmin()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "admin", Email = "admin@test.com" };
        var admin = new Admin { User = user };
        await SeedAsync(user);
        await SeedAsync(admin);

        _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Admin"))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.IsUserAdminByUserIdAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Test]
    public async Task IsUserAdminByUserIdAsync_ShouldReturnFalse_WhenUserIsNotAdmin()
    {
        // Act
        var result = await _repository.IsUserAdminByUserIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}