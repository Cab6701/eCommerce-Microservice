using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // Check if the product already exist 
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} already added");
                }

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity != null && currentEntity.Id > 0)
                {
                    return new Response(true, $"{entity.Name} added to database successfully");
                }
                else
                {
                    return new Response(false, $"Error occurred while adding {entity.Name}");
                }
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                return new Response(false, "Error occurred adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                context.Products.Remove(product);
                context.SaveChanges();
                return new Response(true, $"{entity.Name} is deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                return new Response(false, "Error occurred deleting new product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                return await context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {

                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await context.Products.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> expression)
        {
            try
            {
                return await context.Products.Where(expression).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            if (entity == null)
                return new Response(false, "Error occurred");
            var product = context.Products.Find(entity.Id);
            if (product is null)
                return new Response(false, "Update failed");
            context.SaveChanges();
            context.Entry(product).State = EntityState.Detached;
            context.Products.Update(entity);
            context.SaveChanges();
            return new Response(true, "Update successfully");
        }
    }
}
