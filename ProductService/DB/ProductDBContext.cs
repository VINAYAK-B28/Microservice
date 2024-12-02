using Microsoft.EntityFrameworkCore;
using ProductService.Model;

namespace ProductService.DB
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }


        #region using ExecuteSqlRaw and FromSqlRaw.
        public async Task<List<Product>> ExecuteGetProductsAsync()
        {
            return await Products.FromSqlRaw("EXEC GetProducts").ToListAsync();
        }

        public async Task<Product> ExecuteGetProductByIdAsync(int id)
        {
            return await Products.FromSqlRaw("EXEC GetProductById @Id = {0}", id).FirstOrDefaultAsync();
        }

        public async Task<int> ExecuteAddProductAsync(Product product)
        {
            return await Database.ExecuteSqlRawAsync(
                "EXEC AddProduct @Name = {0}, @Description = {1}, @Price = {2}, @Stock = {3}",
                product.Name, product.Description, product.Price, product.Stock
            );
        }

        public async Task<int> ExecuteUpdateProductAsync(Product product)
        {
            return await Database.ExecuteSqlRawAsync(
                "EXEC UpdateProduct @Id = {0}, @Name = {1}, @Description = {2}, @Price = {3}, @Stock = {4}",
                product.Id, product.Name, product.Description, product.Price, product.Stock
            );
        }

        public async Task<int> ExecuteDeleteProductAsync(int id)
        {
            return await Database.ExecuteSqlRawAsync("EXEC DeleteProduct @Id = {0}", id);
        }
        #endregion
    }

}

