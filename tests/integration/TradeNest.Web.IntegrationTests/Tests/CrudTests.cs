using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

using TradeNest.GCommon;
using TradeNest.Tests.Common;
using TradeNest.Web.IntegrationTests.Models;

namespace TradeNest.Web.IntegrationTests.Tests;

public class CrudTests : WebIntegrationTestsBase
{
    [Test]
    public async Task Get_WorksCorrectly()
    {
        // Arrange
        using HttpClient httpClient = this.Factory.CreateClient();
        
        // Act
        using HttpResponseMessage res = await httpClient.GetAsync("Categories/Index");
        
        // Assert
        Assert.That(res.IsSuccessStatusCode,  Is.True);
        Assert.That(res.Content.Headers.ContentType!.MediaType, Is.EqualTo("text/html"));
        Assert.That(await res.Content.ReadAsStringAsync(), Does.Contain("Categories"));
    }
    
    // part of the seed data:
    // Product { Id = 11111111-1111-1111-1111-111111111111, Name = Wireless Bluetooth Headphones, QuantityInStock = 15 ...}
    [Test]
    public async Task CreateCart_ByAddingAProduct_WorksCorrectly()
    {
        // Arrange
        const string URL = "Cart/AddToCart/11111111-1111-1111-1111-111111111111";
        
        using HttpClient httpClient = this.SetupAuthenticatedHttpClient(
            new WebApplicationFactoryClientOptions() { AllowAutoRedirect = true });
        
        AntiforgeryTokens tokens = await this.Factory.GetAntiforgeryTokensAsync(httpClient);
        httpClient.DefaultRequestHeaders.Add(tokens.HeaderName, tokens.RequestToken);

        Dictionary<string, string> formData = new Dictionary<string, string>()
        {
            [tokens.FormFieldName] = tokens.CookieValue,
            ["quantity"] = "2",
        };
        
        // Act
        using HttpResponseMessage res = await httpClient
            .PostAsync(URL, new FormUrlEncodedContent(formData));
        
        // Assert
        Assert.That(res.IsSuccessStatusCode, Is.True);
        Assert.That(res.Content.Headers.ContentType!.MediaType, Is.EqualTo("text/html"));
        Assert.That(await res.Content.ReadAsStringAsync(), Does.Contain("Wireless Bluetooth Headphones"));
    }

    [Test]
    public async Task Post_RemoveProduct_WorksCorrectly()
    {
        // Arrange
        const string DETAILS_URL = "/Products/Details";
        const string CREATE_URL = "/Products/Create";
        const string DELETE_URL = "/Products/Delete";

        using HttpClient client = this.SetupAuthenticatedHttpClient();
        AntiforgeryTokens tokens = await this.Factory.GetAntiforgeryTokensAsync(client);
        client.DefaultRequestHeaders.Add(tokens.HeaderName, tokens.RequestToken);

        string? createdProductId = await CreateTestProductAsync(CREATE_URL, client, tokens);
        Assert.That(createdProductId, Is.Not.Null);
        
        // Act
        using HttpResponseMessage deleteRes = await client.PostAsync($"{DELETE_URL}/{createdProductId}", null);
        using HttpResponseMessage getRes = await client.GetAsync($"{DETAILS_URL}/{createdProductId}");
        
        // Assert
        Assert.That(getRes.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Post_EditProduct_WorksCorrectly()
    {
        // Arrange
        const string EDIT_URL = "/Products/Edit";
        const string CREATE_URL = "/Products/Create";
        
        using HttpClient client = this.SetupAuthenticatedHttpClient(
            new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = true,
                BaseAddress = new Uri("https://localhost")
            });
        AntiforgeryTokens tokens = await this.Factory.GetAntiforgeryTokensAsync(client);
        client.DefaultRequestHeaders.Add(tokens.HeaderName, tokens.RequestToken);

        Dictionary<string, string> createFormData = GenerateTestProductData(tokens);
        string? createdProductId = await CreateTestProductAsync(CREATE_URL, client, tokens, createFormData);
        Assert.That(createdProductId, Is.Not.Null);

        Dictionary<string, string> editFormData = GenerateTestProductData(tokens: tokens, productId: createdProductId);
        using FormUrlEncodedContent content = new FormUrlEncodedContent(editFormData);
        
        // Act
        using HttpResponseMessage editRes = await client.PostAsync($"{EDIT_URL}", content);
        // NOTE: AllowAutoRedirect is set to true, that is why we don't need to fetch product details view, we are redirected there.
        string productDetailsViewContent = await editRes.Content.ReadAsStringAsync();
            
        // Assert
        Assert.That(editRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(productDetailsViewContent, Does.Contain(editFormData["ProductName"]));
        Assert.That(productDetailsViewContent, Does.Not.Contain(createFormData["ProductName"]));
    }

    private static async Task<string?> CreateTestProductAsync(
        string url,
        HttpClient client,
        AntiforgeryTokens tokens,
        Dictionary<string, string>? formData = null)
    {
        formData ??= GenerateTestProductData(tokens);

        using FormUrlEncodedContent content = new FormUrlEncodedContent(formData);
        using HttpResponseMessage res = await client.PostAsync(url, content);
        
        // AllowAutoRedirect = false => HttpStatusCode.Found ; AllowAutoRedirect = true => HttpStatusCode.OK
        string? newProductId = null;
        if (res.StatusCode == HttpStatusCode.Found)
        {
            newProductId = res.Headers.Location!.ToString()
                .Substring(res.Headers.Location.ToString().LastIndexOf('/') + 1);
        }
        else if (res.StatusCode == HttpStatusCode.OK)
        {
            // NOTE:
            // When AllowAutoRedirect is enabled the HttpResponseMessage is not the response to the initial request;
            // it's the response to the last request in the chain. The redirect handler works like this:
            //
            // 1. Sends POST to /Products/Create.
            // 2. Gets back a 302 with Location: /Products/Details/42.
            // 3. Rewrites the request's RequestUri to /Products/Details/42 and sends again.
            // 4. Gets a 200 Ok, and that response is what gets returned and contained in the HttpResponseMessage object
            // Which means the res.RequestMessage.RequestUri is the request made to get the product details which holds the created product Id
            string productDetailsUrl = res.RequestMessage!.RequestUri!.ToString();
            newProductId = productDetailsUrl.Substring(productDetailsUrl.LastIndexOf('/') + 1);
        }
        
        return newProductId;
    }

    private static Dictionary<string, string> GenerateTestProductData(
        AntiforgeryTokens? tokens = null,
        string? productId = null)
    {
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
        
        if (productId != null)
            formData["ProductId"] = productId;

        if (tokens != null)
            formData.Add(tokens.FormFieldName, tokens.RequestToken);

        return formData;
    }
}