using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.Data.Repository;
using TradeNest.Tests.Common;

namespace TradeNest.Data.Tests.RepositoriesTests;

public class ProductsRepositoryTests : BaseRepositoriesTest
{
    private ProductsRepository _productsRepository;

    [SetUp]
    public async Task SetUpProductsRepository()
    {
        this._productsRepository = new ProductsRepository(this.DbContext);
    }

    [TearDown]
    public async Task TearDownProductsRepository()
    {
        this._productsRepository.Dispose();
    }

    [Test]
    public async Task 
        GetProductDetailsWithRelatedDataAsync_IfProductDoesNotExist_ReturnsNull()
    {
        // Act
        Product? nonExistent = await this._productsRepository
            .GetProductDetailsWithRelatedDataAsync(Guid.NewGuid());

        // Assert
        Assert.IsNull(nonExistent);
    }

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public async Task 
        GetProductDetailsWithRelatedDataAsync_IfProductExistsAndNotArchived_ReturnsProductWithRelatedData(
            bool isApproved,
            bool isEnabled)
    {
        // Arrange
        ApplicationUser owner = new ApplicationUser() { Id = Guid.NewGuid() };
        ApplicationUser adminUser = new ApplicationUser() { Id = Guid.NewGuid() };
        Admin approvalDecisionMaker = new Admin() { Id = Guid.NewGuid(), UserId = adminUser.Id };
        Category category = new Category() { Id = Guid.NewGuid() };
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            OwnerId = owner.Id,
            ApprovalDecisionMakerId = approvalDecisionMaker.Id,
            ApprovalDecision = new ApprovalDecision() 
            {
                ApprovalStatus = isApproved
                    ? ApprovalStatus.Approved
                    : ApprovalStatus.Disapproved
            },
            IsEnabled = isEnabled ? true : false,
            CategoryId = category.Id
        };
        await this.SeedAsync(category);
        await this.SeedAsync(owner);
        await this.SeedAsync(adminUser);
        await this.SeedAsync(approvalDecisionMaker);
        await this.SeedAsync(product);

        // Act
        Product? fetchedProduct = await this._productsRepository
            .GetProductDetailsWithRelatedDataAsync(product.Id);

        // Assert
        Assert.IsNotNull(fetchedProduct);

        Assert.That(
            fetchedProduct.Id,
             Is.EqualTo(product.Id));

        Assert.That(
            fetchedProduct.Category.Id,
             Is.EqualTo(product.CategoryId));

        Assert.That(
            fetchedProduct.ApprovalDecisionMakerId,
             Is.EqualTo(product.ApprovalDecisionMaker!.Id));
    }

    [Test]
    public async Task 
        GetProductDetailsWithRelatedDataAsync_IfProductExistsButArchived_ReturnsNull()
    {
        // Arrange
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Disapproved },
            IsEnabled = false,
            IsDeleted = true,
        };
        await this.SeedAsync(product);

        // Act
        Product? target = await this._productsRepository
            .GetProductDetailsWithRelatedDataAsync(product.Id);

        // Assert
        Assert.IsNull(target);
    }

    [Test]
    public async Task GetAllInclArchivedAndNotApprovedAsync_IfNone_ReturnsEmptyEnumerable()
    {
        // Arrange & Act
        IEnumerable<Product> empty = await this._productsRepository
            .GetAllInclArchivedAndNotApprovedAsync();

        // Assert
        Assert.IsEmpty(empty);
    }

    [TestCase(true, true, true)] [TestCase(false, false, false)]
    [TestCase(true, false, true)] [TestCase(true, true, false)] [TestCase(true, false, false)]
    [TestCase(false, true, true)] [TestCase(true, true, false)] [TestCase(false, true, false)]
    [TestCase(false, true, true)] [TestCase(true, false, true)] [TestCase(false, false, true)]
    public async Task GetAllInclArchivedAndNotApprovedAsync_WorksCorrectly(
        bool isApproved,
        bool isEnabled,
        bool IsDeleted)
    {
        // Arrange
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() 
            {
                ApprovalStatus = isApproved
                    ? ApprovalStatus.Approved
                    : ApprovalStatus.Disapproved
            },
            IsEnabled = isEnabled ? true : false,
            IsDeleted = IsDeleted ? true : false,
        };
        Product inversed = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() 
            {
                ApprovalStatus = isApproved
                    ? ApprovalStatus.Disapproved
                    : ApprovalStatus.Approved
            },
            IsEnabled = isEnabled ? false : true,
            IsDeleted = IsDeleted ? false : true,
        };
        await this.SeedAsync(product);
        await this.SeedAsync(inversed);

        // Act
        IEnumerable<Product> res = (await this._productsRepository
                .GetAllInclArchivedAndNotApprovedAsync())
            .ToList();

        // Assert
        Assert.That(res.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllInclArchivedAndNotApprovedAsync_WithQueryOptions_WorksCorrectly()
    {
        // Arrange
        (
            Product approvedAndEnabledAndPassFilter,
            Product approvedAndEnabledButNoPassFilter,
            Product archivedButPassFilter,
            Product archivedNoPassFilter
        ) = GenerateVaryingProducts(
            out string nameFilter,
            whichProductsContainStringInName: new int[] { 1, 2 }
        );
        await this.SeedAsync(
            approvedAndEnabledAndPassFilter,
            approvedAndEnabledButNoPassFilter,
            archivedButPassFilter,
            archivedNoPassFilter);

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllInclArchivedAndNotApprovedAsync(queryOptions =>
                queryOptions.SetFilter(p => p.Name.Contains(nameFilter)));

        // Assert
        Assert.That(res.Count, Is.EqualTo(2));
        Assert.That(res.All(p => p.Name.Contains(nameFilter)), Is.True);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task 
        GetAllInclNotApprovedAsync_WhenProductDoesNotExistOrArchived_ReturnsEmptyEnumerable(
            bool exists)
    {
        // Arrange
        Product archived = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Disapproved },
            IsEnabled = false,
            IsDeleted = true
        };
        if (exists)
            await this.SeedAsync(archived);

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllInclNotApprovedAsync();

        // Assert
        Assert.IsEmpty(res);
    }

    [Test]
    public async Task GetAllInclNotApprovedAsync_WorksCorrectly()
    {
        // Arrange
        (
            Product approvedAndEnabledAndPassFilter,
            Product approvedAndEnabledButNoPassFilter,
            Product archivedButPassFilter,
            Product archivedNoPassFilter
        ) = GenerateVaryingProducts(out string stringContainedInName, secondHalfArchived: true);
        await this.SeedAsync(
            approvedAndEnabledAndPassFilter,
            approvedAndEnabledButNoPassFilter,
            archivedButPassFilter,
            archivedNoPassFilter);

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllInclNotApprovedAsync();

        // Assert
        Assert.That(res.Count, Is.EqualTo(2));
        Assert.That(res.All(p => p.IsDeleted == false), Is.True);
    }

    [Test]
    public async Task GetAllInclNotApprovedAsync_WithQueryOptions_WorksCorrectly()
    {
        // Arrange
        (
            Product approvedAndEnabledAndPassFilter,
            Product approvedAndEnabledButNoPassFilter,
            Product archivedButPassFilter,
            Product archivedNoPassFilter
        ) = GenerateVaryingProducts(
            out string stringContainedInName,
            secondHalfArchived: true,
            whichProductsContainStringInName: new int[] { 1 }
        );
        await this.SeedAsync(
            approvedAndEnabledAndPassFilter,
            approvedAndEnabledButNoPassFilter,
            archivedButPassFilter,
            archivedNoPassFilter);

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllInclNotApprovedAsync(queryOptions =>
                queryOptions.SetFilter(p => p.Name.Contains(stringContainedInName)));

        // Assert
        Assert.That(res.Count, Is.EqualTo(1));
        Assert.That(
            res.All(p => p.IsDeleted == false && p.Name.Contains(stringContainedInName)),
            Is.True);
    }

    [Test]
    public async Task 
        GetAllCategoriesBestSellersFrontImagesAsync_IfNoProductsInCategories_ReturnsValuesNull()
    {
        // Arrange
        Category firstEmptyCategory = new Category()
        {
            Id = Guid.NewGuid(),
        };
        Category secondEmptyCategory = new Category()
        {
            Id = Guid.NewGuid(),
        };
        await this.SeedAsync(firstEmptyCategory, secondEmptyCategory);

        // Act
        IEnumerable<KeyValuePair<Guid, string?>> emptyCategories = await this._productsRepository
            .GetAllCategoriesBestSellersFrontImagesAsync();

        // Assert
        Assert.That(emptyCategories.All(kvp => kvp.Value == null), Is.True);
    }

    [Test]
    public async Task GetAllCategoriesBestSellersFrontImagesAsync_WorksCorrectly()
    {
        // Arrange
        Category firstCategory = new Category() { Id = Guid.NewGuid(), };
        Category secondCategory = new Category() { Id = Guid.NewGuid(), };

        (Product prod1, Product prod2, Product prod3, Product prod4) 
            = GenerateVaryingProducts(
                out string stringContainedInName,
                secondHalfArchived: false,
                secondHalfDisabled: false,
                secondHalfDisapproved: false);
        AddSalesToProducts(prod1, prod4);
        AddImageToProducts(prod1, prod2, prod3, prod4);

        prod1.CategoryId = firstCategory.Id;
        prod2.CategoryId = firstCategory.Id;
        prod3.CategoryId = secondCategory.Id;
        prod4.CategoryId = secondCategory.Id;

        await this.SeedAsync(firstCategory, secondCategory);
        await this.SeedAsync(prod1, prod2, prod3, prod4);
    
        // Act
        IEnumerable<KeyValuePair<Guid, string?>> categoriesIdsWithBestSellerImages 
            = await this._productsRepository.GetAllCategoriesBestSellersFrontImagesAsync();

        // Assert
        Assert.That(
            categoriesIdsWithBestSellerImages.Count,
            Is.EqualTo(2));
        Assert.That(
            categoriesIdsWithBestSellerImages.Any(kvp => kvp.Value == prod1.Images.First().Url),
            Is.True);
        Assert.That(
            categoriesIdsWithBestSellerImages.Any(kvp => kvp.Value == prod4.Images.First().Url),
            Is.True);
        Assert.That(
            categoriesIdsWithBestSellerImages.Any(kvp => kvp.Value == prod2.Images.First().Url),
            Is.False);
        Assert.That(
            categoriesIdsWithBestSellerImages.Any(kvp => kvp.Value == prod3.Images.First().Url),
            Is.False);
    }

    [Test]
    public async Task GetAllProductsWithCategoryAndImagesAsync_WhenNoProductsExist_ReturnsEmptyEnumerable()
    {
        // Arrange & Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsync();
        
        // Assert
        Assert.IsEmpty(res);
    }

    [Test]
    public async Task GetAllProductsWithCategoryAndImagesAsync_WorksCorrectly()
    {
        // Arrange
        Category category = new Category() { Id = Guid.NewGuid() };
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            CategoryId = category.Id,
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
        };
        AddImageToProducts(product);
        AddImageToProducts(product);

        await this.SeedAsync(category);
        await this.SeedAsync(product);

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsync();

        // Assert
        Assert.That(res.Count, Is.EqualTo(1));
        Assert.That(res.First().CategoryId, Is.EqualTo(category.Id));
        Assert.That(res.First().Images.Count, Is.EqualTo(2));
        Assert.That(res.First().Images.All(i => product.Images.Any(img => i.Id == img.Id)), Is.True);
    }

    [Test]
    public async Task GetAllProductsWithCategoryAndImagesAsync_WithQueryOptions_WorksCorrectly()
    {
        // Arrange
        Category firstCategory = new Category() { Id = Guid.NewGuid(), };
        Category secondCategory = new Category() { Id = Guid.NewGuid(), };

        (Product prod1, Product prod2, Product prod3, Product prod4)
            = GenerateVaryingProducts(
                out string stringContainedInName,
                secondHalfArchived: false,
                secondHalfDisabled: false,
                secondHalfDisapproved: false,
                whichProductsContainStringInName: new int[] { 1, 3 });
        AddImageToProducts(prod1, prod2, prod3, prod4);
        AddImageToProducts(prod1, prod2, prod3, prod4);

        prod1.CategoryId = firstCategory.Id;
        prod2.CategoryId = firstCategory.Id;
        prod3.CategoryId = secondCategory.Id;
        prod4.CategoryId = secondCategory.Id;

        await this.SeedAsync(firstCategory, secondCategory);
        await this.SeedAsync(prod1, prod2, prod3, prod4);

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsync(queryOptions =>
                queryOptions.SetFilter(p => p.Name.Contains(stringContainedInName)));

        // Assert
        Assert.That(res.Count, Is.EqualTo(2));
        Assert.That(res.All(p => p.Images.Any()), Is.True);
        Assert.That(
            res.All(p => p.CategoryId == firstCategory.Id || p.CategoryId == secondCategory.Id),
            Is.True);
    }

    [TestCase(true, true, true)] 
    [TestCase(true, false, true)]
    [TestCase(true, true, false)] 
    [TestCase(true, false, false)]
    [TestCase(false, true, true)] 
    [TestCase(true, true, false)] 
    [TestCase(false, true, true)] 
    [TestCase(true, false, true)] 
    [TestCase(false, false, true)]
    public async Task 
        GetAllProductsWithCategoryAndImagesAsync_WhenProductsDisapprovedOrArchived_DoesNotReturnThem(
            bool halfArchived,
            bool halfDisabled,
            bool halfDisapproved)
    {
        // Arrange
        Category category = new Category() { Id = Guid.NewGuid() };
        (Product prod1, Product prod2, Product prod3, Product prod4)
            = GenerateVaryingProducts(
                out string stringContainedInName,
                halfArchived,
                halfDisabled,
                halfDisapproved);
        AddImageToProducts(prod1, prod2, prod3, prod4);
        prod1.CategoryId = category.Id;
        prod2.CategoryId = category.Id;
        prod3.CategoryId = category.Id;
        prod4.CategoryId = category.Id;

        await this.SeedAsync(category);
        await this.SeedAsync(prod1, prod2, prod3, prod4);

        // prod3 & prod4 - isdeleted = false, isenabled = false, approvalstatus.approved

        // Act
        IEnumerable<Product> res = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsync();
        
        // Assert
        Assert.That(res.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetSpecifiedProductsCountAsync_WhenNoneExist_ReturnsZero()
    {
        // Arrange & Act
        int res = await this._productsRepository.GetSpecifiedProductsCountAsync();

        // Assert
        Assert.That(res, Is.EqualTo(0));
    }

    [Test]
    public async Task GetSpecifiedProductsCountAsync_WorksCorrectly()
    {
        // Arrange
        Category category = new Category() { Id = Guid.NewGuid() };
        (Product prod1, Product prod2, Product prod3, Product prod4)
            = GenerateVaryingProducts(
                out string stringContainedInName,
                secondHalfArchived: true,
                secondHalfDisabled: true,
                secondHalfDisapproved: true,
                whichProductsContainStringInName: new int[] { 1, 2 });
        prod1.ApprovalDecision.ApprovalStatus = ApprovalStatus.WaitingApproval;
        prod1.CategoryId = category.Id;
        prod2.CategoryId = category.Id;
        prod3.CategoryId = category.Id;
        prod4.CategoryId = category.Id;
        await this.SeedAsync(category);
        await this.SeedAsync(prod1, prod2, prod3, prod4);

        // Act
        int res = await this._productsRepository
            .GetSpecifiedProductsCountAsync(search: stringContainedInName);

        // Assert
        Assert.That(res, Is.EqualTo(2));
    }

    [Test]
    public async Task GetSpecifiedProductsCountAsync_WhenTargetingNotApproved_ReturnsOnlyNotApproved()
    {
        // Arrange
        (Product prod1, Product prod2, Product prod3, Product prod4)
            = GenerateVaryingProducts(
                out string stringContainedInName,
                secondHalfArchived: false,
                secondHalfDisabled: false,
                secondHalfDisapproved: true);
        await this.SeedAsync(prod1, prod2, prod3, prod4);

        // Act
        int res = await this._productsRepository
            .GetSpecifiedProductsCountAsync(approved: false);

        // Assert
        Assert.That(res, Is.EqualTo(2));
    }

    [Test]
    public async Task ExistsIncludingArchivedAndNotApprovedAsync_WorksCorrectly()
    {
        // Arrange
        Product archived = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Disapproved },
            IsDeleted = true,
        };
        await this.SeedAsync(archived);

        // Act
        bool res = await this._productsRepository
            .ExistsIncludingArchivedAndNotApprovedAsync(p => p.Id == archived.Id);

        // Assert
        Assert.That(res, Is.True);
    }

    [Test]
    public async Task AddAsync_IfAlreadyExists_ShouldThrow()
    {
        // Arrange
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
        };
        await this.SeedAsync(product);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
           await this._productsRepository.AddAsync(product));
    }

    [Test]
    public async Task AddAsync_WorksCorrectly()
    {
        // Arrange
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
        };

        // Act
        bool res = await this._productsRepository.AddAsync(product);

        // Assert
        Assert.That(this.DbContext.Products.Any(p => p.Id == product.Id), Is.True);
    }

    [Test]
    public async Task AddRangeAsync_WhenProductExists_ShouldThrow()
    {
        // Arrange
        (Product prod1, Product prod2, Product prod3, Product prod4)
            = GenerateVaryingProducts(
                out string stringContainedInName,
                secondHalfArchived: false,
                secondHalfDisabled: false,
                secondHalfDisapproved: false);
        await this.SeedAsync(prod3);
        
        // Act & Assert
        IEnumerable<Product> setupProds = new Product[] { prod1, prod2, prod3, prod4 };
        Assert.ThrowsAsync<ArgumentException>(async () =>
           await this._productsRepository.AddRangeAsync(setupProds));
    }

    [Test]
    public async Task AddRangeAsync_WorksCorrectly()
    {
        // Arrange
        (Product prod1, Product prod2, Product prod3, Product prod4)
            = GenerateVaryingProducts(
                out string stringContainedInName,
                secondHalfArchived: false,
                secondHalfDisabled: false,
                secondHalfDisapproved: false);
        
        // Act
        IEnumerable<Product> setupProds = new Product[] { prod1, prod2, prod3, prod4 };
        bool res = await this._productsRepository.AddRangeAsync(setupProds);

        // Assert
        IEnumerable<Product> dbProds = await this.DbContext.Products.ToArrayAsync();
        Assert.That(dbProds.Count, Is.EqualTo(setupProds.Count()));
    }

    [Test]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldThrow()
    {
        // Arrange
        Product nonExistent = new Product() { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => 
            await this._productsRepository.UpdateAsync(nonExistent));
    }

    [Test]
    public async Task UpdateAsync_WorksCorrectly()
    {
        // Arrange
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.WaitingApproval },
        };
        await this.SeedAsync(product);
    
        Product? fetchedProd = await this.DbContext.Products
            .IgnoreQueryFilters()
            .Include(p => p.ApprovalDecision)
            .SingleOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(fetchedProd);
        Assert.That(fetchedProd.ApprovalDecision.ApprovalStatus, Is.EqualTo(ApprovalStatus.WaitingApproval));

        fetchedProd.ApprovalDecision.ApprovalStatus = ApprovalStatus.Approved;
        this.DbContext.Entry(fetchedProd).State = EntityState.Detached;
    
        // Act
        bool res = await this._productsRepository.UpdateAsync(fetchedProd);
        this.DbContext.Entry(fetchedProd).State = EntityState.Detached;

        // Assert
        Assert.That(res, Is.True);

        Product? newRefToSAmeProd = await this.DbContext.Products
            .Include(p => p.ApprovalDecision)
            .SingleOrDefaultAsync(p => p.Id == product.Id);
        Assert.That(newRefToSAmeProd!.ApprovalDecision.ApprovalStatus, Is.EqualTo(ApprovalStatus.Approved));
    }

    [Test]
    public async Task ArchiveAsync_WorksCorrectly()
    {
        // Arrange
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
            IsDeleted = false,
        };
        await this.SeedAsync(product);
    
        Product? fetchedProd = await this.DbContext.Products
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(fetchedProd);
        Assert.That(fetchedProd.IsDeleted, Is.False);

        // Act
        bool res = await this._productsRepository.ArchiveAsync(fetchedProd);
        this.DbContext.Entry(fetchedProd).State = EntityState.Detached;

        // Assert
        Product? newRefToSAmeProd = await this.DbContext.Products
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == product.Id);
        Assert.That(res, Is.True);
        Assert.IsNotNull(newRefToSAmeProd);
        Assert.That(newRefToSAmeProd.IsDeleted, Is.True);
    }

    private static ValueTuple<Product, Product, Product, Product> GenerateVaryingProducts(
        out string stringContainedInName,
        bool secondHalfArchived = true,
        bool secondHalfDisabled = true,
        bool secondHalfDisapproved = true,
        params int[] whichProductsContainStringInName)
    {
        stringContainedInName = RandomStringGenerator.RandomString(
            minLength: 6,
            maxLength: 100);

        Product prod1 = new Product()
        {
            Id = Guid.NewGuid(),
            Name = whichProductsContainStringInName.Contains(1) 
                ? $"{stringContainedInName}1" 
                : "prod1",
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
            IsEnabled = true,
            IsDeleted = false,
        };
        Product prod2 = new Product()
        {
            Id = Guid.NewGuid(),
            Name = whichProductsContainStringInName.Contains(2) 
                ? $"{stringContainedInName}2" 
                : "prod2",
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
            IsEnabled = true,
            IsDeleted = false,
        };
        Product prod3 = new Product()
        {
            Id = Guid.NewGuid(),
            Name = whichProductsContainStringInName.Contains(3) 
                ? $"{stringContainedInName}3" 
                : "prod3",
            ApprovalDecision = new ApprovalDecision() 
            {
                ApprovalStatus = secondHalfDisapproved
                    ? ApprovalStatus.Disapproved
                    : ApprovalStatus.Approved
            },
            IsEnabled = secondHalfDisabled ? false : true,
            IsDeleted = secondHalfArchived ? true : false,
        };
        Product prod4 = new Product()
        {
            Id = Guid.NewGuid(),
            Name = whichProductsContainStringInName.Contains(4) 
                ? $"{stringContainedInName}4" 
                : "prod4",
            ApprovalDecision = new ApprovalDecision() 
            {
                ApprovalStatus = secondHalfDisapproved
                    ? ApprovalStatus.Disapproved
                    : ApprovalStatus.Approved
            },
            IsEnabled = secondHalfDisabled ? false : true,
            IsDeleted = secondHalfArchived ? true : false,
        };

        return (prod1, prod2, prod3, prod4);
    }

    private static void AddSalesToProducts(params Product[] products)
    {
        foreach (Product product in products)
        {
            product.SoldProducts.Add(new OrderProduct() 
            {
                Id = Guid.NewGuid(),
                OriginalProductId = product.Id,
                QuantityOrdered = 10,
            });
        }
    }

    private static void AddImageToProducts(params Product[] products)
    {
        foreach (Product product in products)
        {
            Image image = new Image()
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Url = $"https://{RandomStringGenerator.RandomString(length: 20)}",
                IsFrontImage = product.Images.Any() ? false : true,
            };
            product.Images.Add(image);
        }
    }
}