using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

using TradeNest.GCommon;
using TradeNest.Tests.Common;
using TradeNest.Web.IntegrationTests.Models;

namespace TradeNest.Web.IntegrationTests.Tests;

public class AuthTests : WebIntegrationTestsBase
{
    [Test]
    public async Task Get_ToResourceThatRequiresAuthenticatedUser_ShouldRedirectToLoginPage()
    {
        // Arrange
        const string URL = "/Products/Create";

        using HttpClient client = this.Factory
            .CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });
        
        // Act
        using HttpResponseMessage res = await client.GetAsync(URL);

        // Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That(res.Headers.Location!.OriginalString.Contains("Identity/Account/Login"), Is.True);
    } 
    
    [Test]
    public async Task Post_CreateProduct_WithAuthenticatedUser_ShouldCreateProduct()
    {
        // Arrange
        const string URL = "/Products/Create";

        using HttpClient client = this.SetupAuthenticatedHttpClient();
        AntiforgeryTokens tokens = await this.Factory.GetAntiforgeryTokensAsync(client);
        client.DefaultRequestHeaders.Add(tokens.HeaderName, tokens.RequestToken);

        Dictionary<string, string> formData = new Dictionary<string, string>()
        {
            [tokens.FormFieldName] = tokens.RequestToken,
            ["ProductName"] = RandomStringGenerator.RandomString(10),
            ["SellingPrice"] = $"{Random.Shared.Next((int)EntityValidationConstants.Product.MinSellingPriceValue, (int)EntityValidationConstants.Product.MaxSellingPriceValue)}",
            ["CostPrice"] = $"{Random.Shared.Next((int)EntityValidationConstants.Product.MinCostPriceValue, (int)EntityValidationConstants.Product.MaxCostPriceValue)}",
            ["CategoryId"] = "1a2b3c4d-5e6f-7890-abcd-ef0123456789", // in the seed data: Category { "Id": "a1b2c3d4-e5f6-7890-1234-567890abcdef", "Name": "Books" },
            ["IsEnabled"] = "True",
            ["QuantityInStock"] = $"{Random.Shared.Next(EntityValidationConstants.Product.MinQuantityInStockValue, EntityValidationConstants.Product.MaxQuantityInStockValue)}",
            ["Description"] = RandomStringGenerator.RandomString(20),
            ["FrontImageUrl"] = $"https://{RandomStringGenerator.RandomString(20)}",
            ["ExtraImagesUrls"] = $"https://{RandomStringGenerator.RandomString(20)}\nhttps://{RandomStringGenerator.RandomString(20)}",
        };
        using FormUrlEncodedContent content = new FormUrlEncodedContent(formData);
        
        // Act
        using HttpResponseMessage res = await client.PostAsync(URL, content);
        using HttpResponseMessage prodDetails = await client.GetAsync(res.Headers.Location);
        
        // Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Found));
        Assert.That(res.Headers.Location!.OriginalString, Does.Contain("Products/Details"));
        Assert.That(await prodDetails.Content.ReadAsStringAsync(), Does.Contain(formData["ProductName"]));
    }

    [Test]
    public async Task Post_WithAuthUserButNoAntiforgeryTokens_ShouldReturnBadRequest()
    {
        // Arrange
        const string URL = "/Products/Create";

        using HttpClient client = this.SetupAuthenticatedHttpClient(
            new WebApplicationFactoryClientOptions() { AllowAutoRedirect = true });
        
        Dictionary<string, string> formData = new Dictionary<string, string>()
        {
            ["ProductName"] = RandomStringGenerator.RandomString(10),
            ["SellingPrice"] = $"{Random.Shared.Next((int)EntityValidationConstants.Product.MinSellingPriceValue, (int)EntityValidationConstants.Product.MaxSellingPriceValue)}",
            ["CostPrice"] = $"{Random.Shared.Next((int)EntityValidationConstants.Product.MinCostPriceValue, (int)EntityValidationConstants.Product.MaxCostPriceValue)}",
            ["CategoryId"] = "1a2b3c4d-5e6f-7890-abcd-ef0123456789", // in the seed data: Category { "Id": "a1b2c3d4-e5f6-7890-1234-567890abcdef", "Name": "Books" },
            ["IsEnabled"] = "True",
            ["QuantityInStock"] = $"{Random.Shared.Next(EntityValidationConstants.Product.MinQuantityInStockValue, EntityValidationConstants.Product.MaxQuantityInStockValue)}",
            ["Description"] = RandomStringGenerator.RandomString(20),
            ["FrontImageUrl"] = $"https://{RandomStringGenerator.RandomString(20)}",
            ["ExtraImagesUrls"] = $"https://{RandomStringGenerator.RandomString(20)}\nhttps://{RandomStringGenerator.RandomString(20)}",
        };
        using FormUrlEncodedContent content = new FormUrlEncodedContent(formData);
        
        // Act
        using HttpResponseMessage res = await client.PostAsync(URL, content);
        
        // Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_RegisterUser_ShouldWorkCorrectly()
    {
        // Arrange
        const string REGISTER_URL = "Identity/Account/Register";
        
        using HttpClient client = this.Factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true,
            BaseAddress = new Uri("https://localhost")
        });
        AntiforgeryTokens tokens = await this.Factory.GetAntiforgeryTokensAsync(client);
        client.DefaultRequestHeaders.Add(tokens.HeaderName, tokens.RequestToken);

        string testPassword = $"{RandomStringGenerator.RandomString(10)}0!";
        Dictionary<string, string> registerFormData = new Dictionary<string, string>()
        {
            [tokens.FormFieldName] = tokens.RequestToken,
            ["Input.UserName"] = RandomStringGenerator.RandomString(10),
            ["Input.Email"] = $"{RandomStringGenerator.RandomString(10)}@gmail.com",
            ["Input.Password"] = testPassword,
            ["Input.ConfirmPassword"] = testPassword,
        };
        using FormUrlEncodedContent registerFormUrlContent = new FormUrlEncodedContent(registerFormData);
        
        // Act
        using HttpResponseMessage res = await client.PostAsync(REGISTER_URL, registerFormUrlContent);
        
        // Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(
            await res.Content.ReadAsStringAsync(),
            Contains.Substring($"Hello {registerFormData["Input.UserName"]}"));
    }
}