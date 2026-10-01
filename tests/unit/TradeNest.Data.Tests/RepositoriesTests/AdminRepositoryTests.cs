using Microsoft.AspNetCore.Identity;

using NUnit.Framework;
using Moq;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using TradeNest.Tests.Common;

namespace TradeNest.Data.Tests.RepositoriesTests;

public class AdminRepositoryTests : BaseRepositoriesTest
{
    private AdminsRepository _adminsRepository;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;

    private IEnumerable<ApplicationUser> _testUsers;
    private IEnumerable<Admin> _testAdmins;

    [SetUp]
    public async Task SetUp()
    {
        Mock<IUserStore<ApplicationUser>> store = new Mock<IUserStore<ApplicationUser>>();
        this._userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        this._adminsRepository = new AdminsRepository(DbContext, _userManagerMock.Object);

        (this._testUsers, this._testAdmins) = SetUpThreeAdminUsers();
        await base.SetUp();
    }

    [TearDown]
    public async Task TearDown()
    {
        this._adminsRepository.Dispose();
        await base.TearDown();
    }

    [Test]
    public async Task GetAllAsync_WithoutQueryOptions_WorksCorrectly()
    {
        // Arrange
        for (int i = 0; i < this._testUsers.Count(); i++)
        {
            await this.SeedAsync(this._testUsers.ElementAt(i));
            await this.SeedAsync(this._testAdmins.ElementAt(i));
        }
        
        // Act
        IEnumerable<Admin> admins = await this._adminsRepository.GetAllAsync();
        
        // Assert
        Assert.IsNotEmpty(admins);
    }

    [Test]
    public async Task GetAllAsync_WithQueryOptions_WorksCorrectly()
    {
        // Arrange
        Guid adminId = Guid.NewGuid();
        Admin admin = new Admin() { Id = adminId, UserId = Guid.NewGuid() };
        await this.SeedAsync(admin);

        //Act
        IEnumerable<Admin> res = await this._adminsRepository
            .GetAllAsync(queryOptions => queryOptions.SetFilter(a => a.Id == adminId));
        
        // Assert
        Assert.That(res.SingleOrDefault()!.Id, Is.EqualTo(admin.Id!));
    }

    [Test]
    public async Task FindByIdAsync_IfAdminWithTargetIdDoesNotExist_ReturnsNull()
    {
        // Arrange
        Guid newId = Guid.NewGuid();
        
        // Act
        Admin? nonExistent = await this._adminsRepository.FindByIdAsync(newId);
        
        // Assert
        Assert.IsNull(nonExistent);
    }

    [Test]
    public async Task FindByIdAsync_IfAdminWithTargetIdExists_ReturnsAdmin()
    {
        // Arrange
        Guid newId = Guid.NewGuid();
        Admin admin = new Admin()
        {
            Id = newId,
            UserId = Guid.NewGuid()
        };
        await this.SeedAsync(admin);
        
        // Act
        Admin? res = await this._adminsRepository.FindByIdAsync(admin.Id);
        
        // Assert
        Assert.IsNotNull(res);
        Assert.That(res.Id, Is.EqualTo(newId));
    }

    [Test]
    public async Task ExistsAsync_IfAdminWithTargetIdExists_ReturnsTrue()
    {
        // Arrange
        Guid newId = Guid.NewGuid();
        Admin admin = new Admin()
        {
            Id = newId,
            UserId = Guid.NewGuid()
        };
        await this.SeedAsync(admin);
        
        // Act
        bool exists = await this._adminsRepository
            .ExistsAsync(a => a.Id == newId);
        
        // Assert
        Assert.That(exists, Is.True);
    }

    [Test]
    public async Task ExistsAsync_IfAdminWithTargetIdDoesNotExists_ReturnsFalse()
    {
        // Arrange
        Guid newId = Guid.NewGuid();
        
        // Act
        bool exists = await this._adminsRepository
            .ExistsAsync(a => a.Id == newId);
        
        // Asset
        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task GetAdminByUserId_WhenUserIsNotAdmin_ReturnsNull()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        ApplicationUser userNotAdmin = new ApplicationUser()
        {
            Id = userId,
            UserName = RandomStringGenerator.RandomString(length: 10),
        };
        await this.SeedAsync(userNotAdmin);
        
        // Act
        Admin? nonExistent = await this._adminsRepository
            .GetAdminByUserId(userId);
        
        // Assert
        Assert.IsNull(nonExistent);
    }
    
    [Test]
    public async Task GetAdminByUserId_WhenUserIsAdmin_ReturnsAdmin()
    {
        // Arrange
        for (int i = 0; i < this._testUsers.Count(); i++)
        {
            await this.SeedAsync(this._testUsers.ElementAt(i));
            await this.SeedAsync(this._testAdmins.ElementAt(i));
        }

        // Act
        Admin? res = await this._adminsRepository
            .GetAdminByUserId(this._testUsers.First().Id);
        
        // Assert
        Assert.IsNotNull(res);
        Assert.That(res.Id, Is.EqualTo(this._testAdmins.First().Id));
        Assert.That(res.UserId == this._testAdmins.First().UserId);
    }

    [Test]
    public async Task IsUserAdminByUserIdAsync_WhenUserIsNotAdmin_ReturnsFalse()
    {
        this._userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        bool isUserAdmin = await this._adminsRepository
            .IsUserAdminByUserIdAsync(Guid.NewGuid());
        
        Assert.IsFalse(isUserAdmin);
    }

    [Test]
    public async Task IsUserAdminByUserIdAsync_WhenUserIsAdmin_ReturnsTrue()
    {
        // Arrange
        await this.SeedAsync(this._testUsers.First());
        
        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        bool isUserAdmin = await this._adminsRepository
            .IsUserAdminByUserIdAsync(this._testUsers.First().Id);

        // Assert
        Assert.IsTrue(isUserAdmin);
    }
    
    [Test]
    public async Task IsUserAdminAsync_WhenUserIsNotAdmin_ReturnsFalse()
    {
        // Arrange
        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        bool isUserAdmin = await this._adminsRepository
            .IsUserAdminAsync(new ApplicationUser() { Id = Guid.NewGuid() });
        
        // Assert
        Assert.IsFalse(isUserAdmin);
    }

    [Test]
    public async Task IsUserAdminAsync_WhenUserIsAdmin_ReturnsTrue()
    {
        // Arrange
        this._userManagerMock
            .Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        bool isUserAdmin = await this._adminsRepository
            .IsUserAdminAsync(this._testUsers.First());

        Assert.IsTrue(isUserAdmin);
    }

    [Test]
    public async Task AddAsync_WhenAdminAlreadyExists_ShouldReturnFalse()
    {
        // Arrange
        this._userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        // Act 
        bool res = await this._adminsRepository.AddAsync(new Admin() { User = It.IsAny<ApplicationUser>() });
        
        // Assert
        Assert.IsFalse(res);
    }

    [Test]
    public async Task AddAsync_WorksCorrectly()
    {
        // Arrange
        for (int i = 0; i < this._testUsers.Count(); i++)
        {
            await this.SeedAsync(this._testUsers.ElementAt(i));
            await this.SeedAsync(this._testAdmins.ElementAt(i));
        }
        
        this._userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        bool res = await this._adminsRepository
            .AddAsync(this._testAdmins.First());
            
        // Assert
        Assert.IsTrue(res);
        Admin? fetchedAdmin = await this._adminsRepository
            .GetAdminByUserId(this._testAdmins.First().UserId);
        Assert.That(fetchedAdmin, Is.Not.Null);
        Assert.That(fetchedAdmin, Is.EqualTo(this._testAdmins.First()));
    }

    private ValueTuple<IEnumerable<ApplicationUser>, IEnumerable<Admin>> SetUpThreeAdminUsers()
    {
        IEnumerable<ApplicationUser> users = new ApplicationUser[]
        {
            new ApplicationUser() { Id = Guid.NewGuid() },
            new ApplicationUser() { Id = Guid.NewGuid() },
            new ApplicationUser() { Id = Guid.NewGuid() }
        };
        IEnumerable<Admin> admins = new Admin[]
        {
            new Admin() { Id = Guid.NewGuid(), UserId = users.ElementAt(0).Id, User = users.ElementAt(0) },
            new Admin() { Id = Guid.NewGuid(), UserId = users.ElementAt(1).Id, User = users.ElementAt(1) },
            new Admin() { Id = Guid.NewGuid(), UserId = users.ElementAt(2).Id, User = users.ElementAt(2) },
        };
        
        return (users, admins);
    }
}