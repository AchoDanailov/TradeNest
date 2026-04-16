using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Product;
using TradeNest.Web.ViewModels.Product;
using TradeNest.Web.Mappers.Interfaces;

namespace TradeNest.Web.Controllers;

public class ProductsApiController : BaseApiController
{
    private readonly IProductsService _productsService;
    private readonly IProductPresentationModelsMapper _productsMapper;

    public ProductsApiController(IProductsService productsService,
        IProductPresentationModelsMapper productsMapper)
    {
        this._productsService = productsService;
        this._productsMapper = productsMapper;
    }

    [HttpGet]
    [Route("products/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    public async Task<ActionResult<ProductDetailsResponseDto>> GetProductDetailsByIdAsync(
        [FromRoute] string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid idGuidValue))
            return BadRequest();
        
        Guid userId = this.GetUserId(throwIfNull: true);
            
        ProductDetailsDto? productDetailsDto = await this._productsService
            .GetProductDetailsByIdAsync(idGuidValue, userId);
        if (productDetailsDto == null)
            return NotFound();
            
        ProductDetailsResponseDto productDetailsResponseDto = this._productsMapper
            .ToProductResponseDto(productDetailsDto);
            
        return Ok(productDetailsResponseDto);
    }

    [HttpGet]
    [Route("products/count")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> GetProductsCountAsync(
        [FromQuery] bool? approved = null,
        [FromQuery] string? search = null)
    {
        Guid userId = this.GetUserId(throwIfNull: true);
        int productsCount = await this._productsService
            .GetSpecifiedProductsCountAsync(userId, approved, search);
        
        return Ok(productsCount);
    }

    [HttpDelete]
    [Route("products/{id}")]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(StatusCodes.Status403Forbidden)] 
    public async Task<ActionResult<bool>> RemoveProductByIdAsync([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out Guid validIdGuidValue))
            return BadRequest();

        Guid userId = GetUserId(throwIfNull: true);
        await this._productsService.DeleteProductAsync(userId, validIdGuidValue);
        return Ok(true);
    }

    [HttpGet]
    [Route("products")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsDataAsync(
        [FromQuery] int? page = null,
        [FromQuery] int? limit = null,
        [FromQuery] bool? approved = null,
        [FromQuery] string? search = null)
    {
        Guid userId = this.GetUserId(throwIfNull: true);
    
        IEnumerable<ProductDto2> productDtos;
        if (page != null && limit != null)
        {
            productDtos = await this._productsService
                .GetProductsDataWithPagination(userId, page.Value, limit.Value, approved, search);
        }
        else
        {
            productDtos = await this._productsService
                .GetProductsData(userId, approved, search);
        }
    
        IEnumerable<ProductResponseDto> productResponseDtos 
            = this._productsMapper.ToProductResponseDtos(productDtos);
        return Ok(productResponseDtos);
    }
}