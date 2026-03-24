using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Convertions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct productInterface) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products detected in the database");

            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound("Product requested not found");

            // Convert from entity to DTO and return
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return  _product is not null ? Ok(_product) : NotFound("No product found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // Check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : NotFound(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // Check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : NotFound(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            // Check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : NotFound(response);
        }
    }
}
