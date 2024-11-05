using ECommerce.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Mappers;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products detected in the database");

            var (_, list) = ProductMapper.ToDto(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No Product Found");
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            var (dto, _) = ProductMapper.ToDto(product, null!);
            return dto is not null ? Ok(dto) : NotFound("Product not found");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDto product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = ProductMapper.ToEntity(product);
            var response = await productInterface.CreateAsync(entity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(int id, ProductDto product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = ProductMapper.ToEntity(product);
            var response = await productInterface.UpdateAsync(entity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDto product)
        {
            var getEntity = ProductMapper.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

    }
}
