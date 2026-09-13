using System.Net;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

using TradeNest.Tests.Common;
using TradeNest.Web.IntegrationTests.Common;
using TradeNest.Web.IntegrationTests.Models;
using TradeNest.Web.IntegrationTests.TestsServices;

namespace TradeNest.Web.IntegrationTests.Tests;

public class AuthTests : WebIntegrationTestsBase
{
    [Test]
    public async Task Get_ToResourceThatRequiresAuthenticatedUser_ShouldRedirectToLoginPage()
    {
        const string URL = "/Products/Create";

        using HttpClient client = this.Factory
            .CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });
        using HttpResponseMessage res = await client.GetAsync(URL);

        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That(res.Headers.Location!.OriginalString.Contains("Identity/Account/Login"), Is.True);
    } 
    
    [Test]
    public async Task Post_CreateProduct_WithAuthenticatedUser_ShouldCreateProduct()
    {
        const string URL = "/Products/Create";

        using HttpClient client = this.SetupAuthenticatedHttpClient();
        AntiforgeryTokens tokens = await this.Factory.GetAntiforgeryTokensAsync(client);
        client.DefaultRequestHeaders.Add(tokens.HeaderName, tokens.RequestToken);

        Dictionary<string, string> formData = new Dictionary<string, string>()
        {
            [tokens.FormFieldName] = tokens.RequestToken,
            ["ProductName"] = RandomStringGenerator.RandomString(10),
            ["SellingPrice"] = "10",
            ["CostPrice"] = "10",
            ["CategoryId"] = "1a2b3c4d-5e6f-7890-abcd-ef0123456789",
            ["IsEnabled"] = "True",
            ["QuantityInStock"] = "5",
            ["Description"] = RandomStringGenerator.RandomString(20),
            ["FrontImageUrl"] = "https://example.com/front.jpg",
            ["ExtraImagesUrls"] = "https://example.com/extra1.jpg\nhttps://example.com/extra2.jpg",
        };

        using FormUrlEncodedContent content = new FormUrlEncodedContent(formData);
        using HttpResponseMessage res = await client.PostAsync(URL, content);
        using HttpResponseMessage prodDetails = await client.GetAsync(res.Headers.Location);
        
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Found));
        Assert.That(res.Headers.Location!.OriginalString, Does.Contain("Products/Details"));
        Assert.That(await prodDetails.Content.ReadAsStringAsync(), Does.Contain(formData["ProductName"]));
    }

    [Test]
    public async Task Post_WithAuthUserButNoAntiforgeryTokens_ShouldReturnBadRequest()
    {
        const string URL = "/Products/Create";

        using HttpClient client = this.SetupAuthenticatedHttpClient(
            new WebApplicationFactoryClientOptions() { AllowAutoRedirect = true });
        
        Dictionary<string, string> formData = new Dictionary<string, string>()
        {
            ["ProductName"] = RandomStringGenerator.RandomString(10),
            ["SellingPrice"] = "10",
            ["CostPrice"] = "10",
            ["CategoryId"] = "1a2b3c4d-5e6f-7890-abcd-ef0123456789",
            ["IsEnabled"] = "True",
            ["QuantityInStock"] = "5",
            ["Description"] = RandomStringGenerator.RandomString(20),
            ["FrontImageUrl"] = "https://example.com/front.jpg",
            ["ExtraImagesUrls"] = "https://example.com/extra1.jpg\nhttps://example.com/extra2.jpg",
        };
        using FormUrlEncodedContent content = new FormUrlEncodedContent(formData);
        using HttpResponseMessage res = await client.PostAsync(URL, content);
        
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_RegisterUser_ShouldWorkCorrectly()
    {
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
        
        using HttpResponseMessage res = await client.PostAsync(REGISTER_URL, registerFormUrlContent);
        
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(
            await res.Content.ReadAsStringAsync(),
            Contains.Substring($"Hello {registerFormData["Input.UserName"]}"));
    }
    
    private HttpClient SetupAuthenticatedHttpClient(
        WebApplicationFactoryClientOptions? clientOptions = null)
    {
        HttpClient client = this.Factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: TestsConstants.Auth.Scheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestsConstants.Auth.Scheme, options => { });
                });
            })
            .CreateClient(clientOptions ?? new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });    
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: TestsConstants.Auth.Scheme);

        return client;
    }
}