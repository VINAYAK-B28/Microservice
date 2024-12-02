using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.DB;
using ProductService.Model;

namespace ProductService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _dbContext;
        //public DbSet<Product> Products { get; set; }

        public ProductController(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Using SP with ExecuteStoredProc Methods

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProductsSP()
        {
            var products = await _dbContext.LoadStoredProc("GetProducts")
                                           .ExecuteStoredProc<Product>();
            return Ok(products);
        }

        [HttpGet("GetProductById{id}")]
        public async Task<IActionResult> GetProductByIdSP(int id)
        {
            var products = await _dbContext.LoadStoredProc("GetProductById")
                                           .WithSqlParam("Id", id)
                                           .ExecuteStoredProc<Product>();
            var product = products.FirstOrDefault();

            if (product == null) return NotFound("Product not found.");
            return Ok(product);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProductSP([FromBody] Product product)
        {
            var result = await _dbContext.LoadStoredProc("AddProduct")
                                         .WithSqlParam("Name", product.Name)
                                         .WithSqlParam("Description", product.Description)
                                         .WithSqlParam("Price", product.Price)
                                         .WithSqlParam("Stock", product.Stock)
                                         .ExecuteStoredProc<dynamic>();

            return Ok(new { status = 200, message = "Product Added Successfully" });
        }

        //[HttpPut("UpdateProductSP{id}")]
        //public async Task<IActionResult> UpdateProductSP(int id, [FromBody] Product updatedProduct)
        //{
        //    await _dbContext.LoadStoredProc("UpdateProduct")
        //                    .WithSqlParam("Id", id)
        //                    .WithSqlParam("Name", updatedProduct.Name)
        //                    .WithSqlParam("Description", updatedProduct.Description)
        //                    .WithSqlParam("Price", updatedProduct.Price)
        //                    .WithSqlParam("Stock", updatedProduct.Stock)
        //                    .ExecuteStoredProc<dynamic>();

        //    return NoContent();
        //}

        //[HttpDelete("DeleteProductSP{id}")]
        //public async Task<IActionResult> DeleteProductSP(int id)
        //{
        //    await _dbContext.LoadStoredProc("DeleteProduct")
        //                    .WithSqlParam("Id", id)
        //                    .ExecuteStoredProc<dynamic>();

        //    return NoContent();
        //}

        #endregion

        #region Without Sp
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _dbContext.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found.");

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        //{
        //    var product = await _dbContext.Products.FindAsync(id);
        //    if (product == null) return NotFound("Product not found.");

        //    product.Name = updatedProduct.Name;
        //    product.Description = updatedProduct.Description;
        //    product.Price = updatedProduct.Price;
        //    product.Stock = updatedProduct.Stock;

        //    await _dbContext.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    var product = await _dbContext.Products.FindAsync(id);
        //    if (product == null) return NotFound("Product not found.");

        //    _dbContext.Products.Remove(product);
        //    await _dbContext.SaveChangesAsync();
        //    return NoContent();
        //}
        #endregion

        #region Using SP with ExecuteSqlRaw and FromSqlRaw.

        [HttpGet("GetProductsRaw")]
        public async Task<IActionResult> GetProductsRaw()
        {
            var products = await _dbContext.ExecuteGetProductsAsync();
            return Ok(products);
        }

        [HttpGet("GetProductByIdRaw{id}")]
        public async Task<IActionResult> GetProductByIdRaw(int id)
        {
            var product = await _dbContext.ExecuteGetProductByIdAsync(id);
            //var productrs = Products.FromSqlRaw("EXEC GetProducts").ToListAsync();
            if (product == null) return NotFound("Product not found.");

            return Ok(product);
        }

        [HttpPost("AddProductRaw")]
        public async Task<IActionResult> AddProductRaw([FromBody] Product product)
        {
            var result = await _dbContext.ExecuteAddProductAsync(product);
            if (result > 0)
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);

            return BadRequest("Failed to add product.");
        }

        //[HttpPut("UpdateProductRaw{id}")]
        //public async Task<IActionResult> UpdateProductRaw(int id, [FromBody] Product updatedProduct)
        //{
        //    updatedProduct.Id = id; // Ensure the ID is set
        //    var result = await _dbContext.ExecuteUpdateProductAsync(updatedProduct);
        //    if (result > 0) return NoContent();

        //    return NotFound("Product not found.");
        //}

        //[HttpDelete("DeleteProductRaw{id}")]
        //public async Task<IActionResult> DeleteProductRaw(int id)
        //{
        //    var result = await _dbContext.ExecuteDeleteProductAsync(id);
        //    if (result > 0) return NoContent();

        //    return NotFound("Product not found.");
        //}
        #endregion
    }
}
