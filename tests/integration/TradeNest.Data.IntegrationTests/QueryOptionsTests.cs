using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;
using TradeNest.Data.IntegrationTests.Utils;
using TradeNest.Data.Models;
using TradeNest.Data.QueryOptions;
using TradeNest.Data.Repository;
using TradeNest.Data.IntegrationTests.Utils;
using TradeNest.Data.Models.Enums;

namespace TradeNest.Data.IntegrationTests;

public class QueryOptionsTests : IntegrationTestsBase
{
    [Test]
    public async Task SetFilter_WorksCorrectly()
    {
        // Arrange
        (Product product, Category category, ApplicationUser user) 
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        (Product randomProd, Category randomCategory, ApplicationUser randomUser) 
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        
        await this.SeedAsync(user, randomUser);
        await this.SeedAsync(category, randomCategory);
        await this.SeedAsync(product, randomProd);

        Action<QueryOptions<Product>> action = (queryOptionsBuilder) =>
            queryOptionsBuilder.SetFilter(p => p.Id == product.Id);
        IQueryable<Product> queryable = QueryOptionsTranslator<Product>
            .ToQueryable(this.DbContext.Products, action);
        
        // Act 
        Product? target = await queryable.SingleOrDefaultAsync();
        
        // Assert
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Id, Is.EqualTo(product.Id));
    }

    [Test]
    public async Task AddOrder_DirectionsDescAndAscTogether_WorksCorrectly()
    {
        // Arrange
        (ApplicationUser user, Order[] orders) = SetupTestOrders();
        
        await this.SeedAsync(user);
        await this.SeedAsync(orders[1]);
        await this.SeedAsync(orders[3]);
        await this.SeedAsync(orders[0]);
        await this.SeedAsync(orders[2]);
        
        Action<QueryOptions<Order>> action = (queryOptionsBuilder) =>
            queryOptionsBuilder
                .AddOrderDesc(o => o.SubmittedOn.Year)
                .AddOrderAsc(o => o.TotalPrice);
        
        IQueryable<Order> queryable = QueryOptionsTranslator<Order>
            .ToQueryable(this.DbContext.Orders, action);
        
        // Act
        Order[] targets = await queryable.ToArrayAsync();

        // Assert
        Assert.That(targets[0].Id, Is.EqualTo(orders[2].Id)); // most recent date (2025)
        Assert.That(targets[1].Id, Is.EqualTo(orders[1].Id)); // date: 2020; totalprice: 2
        Assert.That(targets[2].Id, Is.EqualTo(orders[3].Id)); // date: 2020; totalprice: 4
        Assert.That(targets[3].Id, Is.EqualTo(orders[0].Id)); // date 2000
    }

    [Test]
    public async Task AsReadOnly_OptimisesFetchingCorrectly()
    {
        // Arrange
        (Product product, Category category, ApplicationUser user) 
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        
        await this.SeedAsync(user);
        await this.SeedAsync(category);
        await this.SeedAsync(product);
        
        Action<QueryOptions<Product>> action = (queryOptionsBuilder) =>
            queryOptionsBuilder.AsReadOnly();
        IQueryable<Product> queryable = QueryOptionsTranslator<Product>
            .ToQueryable(this.DbContext.Products, action);
        
        // Act
        Product? target = await queryable.SingleOrDefaultAsync();
        EntityEntry<Product> entry = this.DbContext.Entry<Product>(target!);
        
        // Assert
        Assert.That(entry.State, Is.EqualTo(EntityState.Detached));
    }

    [Test]
    public async Task WithRelated_IncludesRelatedEntitiesCorrectly()
    {
        // Arrange
        (Product product, Category category, ApplicationUser user) 
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        
        await this.SeedAsync(user);
        await this.SeedAsync(category);
        await this.SeedAsync(product);

        Action<QueryOptions<Product>> action = (queryOptionsBuilder) =>
            queryOptionsBuilder
                .WithRelated(p => p.Category)
                .WithRelated(p => p.Owner);
        
        IQueryable<Product> queryable = QueryOptionsTranslator<Product>
            .ToQueryable(this.DbContext.Products, action);
        
        // Act
        Product? target = await queryable.SingleOrDefaultAsync();
        
        // Assert
        Assert.That(target!.Owner.UserName, Is.EqualTo(user.UserName));
        Assert.That(target.Category.Name, Is.EqualTo(category.Name));
    }

    [Test]
    public async Task QueryOptions_WithRepositories_WorksCorrectly()
    {
        // Arrange 
        (Product product, Category category, ApplicationUser user) firstEntities // the 3 products are with the same category
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        firstEntities.product.ApprovalDecision.ApprovalStatus = ApprovalStatus.Disapproved;  // first product is not approved so we can validate the repository method includes not approved
        
        (Product product, ApplicationUser user) secondEntities 
            = TestDataGenerator.GetRandomProductWithOwner(firstEntities.category);
        
        (Product product, ApplicationUser user) thirdEntities 
            = TestDataGenerator.GetRandomProductWithOwner(firstEntities.category);
        thirdEntities.product.IsDeleted = true; // third product is soft deleted so we can validate the repository method doesn't include soft deleted products

        await this.SeedAsync(firstEntities.category);
        await this.SeedAsync(firstEntities.user, secondEntities.user, thirdEntities.user);
        await this.SeedAsync(firstEntities.product, secondEntities.product, thirdEntities.product);
        
        // clearing the change tracker so we can later validate QueryOptions AsReadOnly() work when passed to a repository method
        this.DbContext.ChangeTracker.Clear();
        
        ProductsRepository productsRepository = new ProductsRepository(this.DbContext);
        
        // Act
        IEnumerable<Product> result = (await productsRepository
                .GetAllInclNotApprovedAsync(queryOptions =>
                {
                    queryOptions
                        .AsReadOnly()
                        .WithRelated(p => p.Category)
                        .SetFilter(p => p.CategoryId == firstEntities.category.Id);
                }))
            .ToArray();
        
        // Assert
        Assert.That(result.Count(), Is.EqualTo(2)); // validating the repo method is not broken when passing query options
        Assert.That(result.All(p => p.CategoryId == firstEntities.category.Id), Is.True); // validating the repo method is not broken when passing query options
        Assert.That(result.All(p => p.Category.Name == firstEntities.category.Name), Is.True); // validating QueryOptions "WithRelated()" works when passed to repo method

        bool productsTrackedByChangeTracker = this.DbContext.ChangeTracker
            .Entries<Product>()
            .Any(e => e.Entity.Id == firstEntities.product.Id || e.Entity.Id == secondEntities.product.Id);
        Assert.That(productsTrackedByChangeTracker, Is.False); // => validating "AsReadOnly()" does its job correctly
    }
    
    private static (ApplicationUser user, Order[] orders) SetupTestOrders()
    {
        ApplicationUser user = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName =  Guid.NewGuid().ToString(),
        };
        
        Order firstOrder = new Order()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TotalPrice = 1m,
            SubmittedOn = new DateTime(2000, 1, 1),
        };
        Order secondOrder = new Order()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TotalPrice = 2m,
            SubmittedOn = new DateTime(2020, 1, 1),
        };
        Order thirdOrder = new Order()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TotalPrice = 3m,
            SubmittedOn = new DateTime(2025, 1, 1),
        };
        Order fourthOrder = new Order() // same SubmittedOn date as secondOrder
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TotalPrice = 4m,
            SubmittedOn = new DateTime(2020, 1, 1),
        };
        
        return (user, new Order[] { firstOrder, secondOrder, thirdOrder, fourthOrder });
    }
}