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
    [ProducesResponseType(StatusCodes.Status200OK)] [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponseDto>> GetProductData([FromRoute] string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid idGuidValue))
            return BadRequest();
        
        Guid userId = this.GetUserId(throwIfNull: true);
            
        ProductDetailsDto? productDetailsDto = await this._productsService
            .GetProductDetailsByIdAsync(idGuidValue, userId);
        if (productDetailsDto == null)
            return NotFound();
            
        ProductResponseDto productResponseDto = this._productsMapper
            .ToProductResponseDto(productDetailsDto);
            
        return Ok(productResponseDto);
    }
}