using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using TradeNest.Tests.Common;

namespace TradeNest.Data.IntegrationTests.RepositoriesTests;

public class UsersRepositoryTests : IntegrationTestsBase
{
    private UsersRepository _usersRepository;
    private Mock<UserManager<ApplicationUser>> _userManager;
    private Mock<RoleManager<ApplicationRole>> _roleManager;
    
    [SetUp]
    public void SetUp()
    {
        Mock<IUserStore<ApplicationUser>> userStore = new Mock<IUserStore<ApplicationUser>>();
        Mock<IRoleStore<ApplicationRole>> roleStore = new Mock<IRoleStore<ApplicationRole>>();
        this._userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        this._roleManager = new Mock<RoleManager<ApplicationRole>>(roleStore.Object, null!, null!, null!, null!);
        
        this._usersRepository = new UsersRepository(this.DbContext, this._userManager.Object,
            this._roleManager.Object);
    }

    [TearDown]
    public void TearDown()
    {
        this._usersRepository.Dispose();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ExistsByIdWithForgottenIncludedAsync_WorksCorrectly(bool isForgotten)
    {
        // Arrange
        ApplicationUser user = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            PersonalInformationIsDeleted = isForgotten,
        };
        await this.SeedAsync(user);
        
        // Act 
        bool exists = await this._usersRepository.ExistsByIdWithForgottenIncludedAsync(user.Id);
        
        // Assert
        Assert.That(exists, Is.True);
    }

    [TestCase(null)]
    [TestCase("Admin")]
    public async Task DeleteAsync_WorksCorrectly(string? role)
    {
        // Arrange
        string email = $"{RandomStringGenerator.RandomString(10)}@test.com";
        string userName = RandomStringGenerator.RandomString(6, 15);
        ApplicationUser user = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            PersonalInformationIsDeleted = false,
            Email = email,
            UserName = userName,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = userName.ToUpper(),
            PasswordHash = Guid.NewGuid().ToString(),
        };
        await this.SeedAsync(user);
        
        // Act
        await this._usersRepository.DeleteAsync(user);
        
        // Assert
        ApplicationUser foundUser = (await this.DbContext.Users.FindAsync(user.Id))!;
        bool userHasRole = this.DbContext.UserRoles.Any(r => r.UserId == foundUser.Id);
        bool isUserForgotten = foundUser!.PersonalInformationIsDeleted;
        
        Assert.That(foundUser, Is.Not.Null);
        Assert.That(userHasRole, Is.False);
        Assert.That(isUserForgotten, Is.True);
    }
}