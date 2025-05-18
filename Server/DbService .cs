using CommonData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    public class DbService
    {
        private readonly CookBookDbContext _dbContext;

        public DbService()
        {
            _dbContext = new CookBookDbContext();
        }

        public async Task<Response> ProcessRequestAsync(Request request)
        {
            try
            {
                switch (request.Action)
                {
                    case "GetAll":
                    return await GetAllEntitiesAsync(request.EntityType);
                    case "Add":
                    return await AddEntityAsync(request.EntityType, request.Data);
                    case "Update":
                    return await UpdateEntityAsync(request.EntityType, request.Data);
                    case "Delete":
                    return await DeleteEntityAsync(request.EntityType, request.Data);
                    default:
                    return new Response { Success = false, Error = "Unknown action" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Error = ex.Message };
            }
        }

        private async Task<Response> GetAllEntitiesAsync(string entityType)
        {
            switch (entityType)
            {
                case "Dish":
                var dishes = await _dbContext.Dishes.ToListAsync();
                return new Response { Success = true, Data = JsonConvert.SerializeObject(dishes) };
                case "Tag":
                var tags = await _dbContext.Tags.ToListAsync();
                return new Response { Success = true, Data = JsonConvert.SerializeObject(tags) };
                case "Product":
                var products = await _dbContext.Products.ToListAsync();
                return new Response { Success = true, Data = JsonConvert.SerializeObject(products) };
                default:
                return new Response { Success = false, Error = "Unknown entity type" };
            }
        }

        private async Task<Response> AddEntityAsync(string entityType, string data)
        {
            switch (entityType)
            {
                case "Dish":
                var dish = JsonConvert.DeserializeObject<Dish>(data);
                _dbContext.Dishes.Add(dish);
                await _dbContext.SaveChangesAsync();
                return new Response { Success = true };
                case "Tag":
                var tag = JsonConvert.DeserializeObject<Tag>(data);
                _dbContext.Tags.Add(tag);
                await _dbContext.SaveChangesAsync();
                return new Response { Success = true };
                case "Product":
                var product = JsonConvert.DeserializeObject<Product>(data);
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
                return new Response { Success = true };
                default:
                return new Response { Success = false, Error = "Unknown entity type" };
            }
        }

        private async Task<Response> UpdateEntityAsync(string entityType, string data)
        {
            switch (entityType)
            {
                case "Dish":
                var dish = JsonConvert.DeserializeObject<Dish>(data);
                _dbContext.Dishes.Update(dish);
                await _dbContext.SaveChangesAsync();
                return new Response { Success = true };
                case "Tag":
                var tag = JsonConvert.DeserializeObject<Tag>(data);
                _dbContext.Tags.Update(tag);
                await _dbContext.SaveChangesAsync();
                return new Response { Success = true };
                case "Product":
                var product = JsonConvert.DeserializeObject<Product>(data);
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                return new Response { Success = true };
                default:
                return new Response { Success = false, Error = "Unknown entity type" };
            }
        }

        private async Task<Response> DeleteEntityAsync(string entityType, string data)
        {
            switch (entityType)
            {
                case "Dish":
                var dishId = JsonConvert.DeserializeObject<int>(data);
                var dish = await _dbContext.Dishes.FindAsync(dishId);
                if (dish != null)
                {
                    _dbContext.Dishes.Remove(dish);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                return new Response { Success = false, Error = "Dish not found" };
                case "Tag":
                var tagId = JsonConvert.DeserializeObject<int>(data);
                var tag = await _dbContext.Tags.FindAsync(tagId);
                if (tag != null)
                {
                    _dbContext.Tags.Remove(tag);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                return new Response { Success = false, Error = "Tag not found" };
                case "Product":
                var productId = JsonConvert.DeserializeObject<int>(data);
                var product = await _dbContext.Products.FindAsync(productId);
                if (product != null)
                {
                    _dbContext.Products.Remove(product);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                return new Response { Success = false, Error = "Product not found" };
                default:
                return new Response { Success = false, Error = "Unknown entity type" };
            }
        }
    }
}
