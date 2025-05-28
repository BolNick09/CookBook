using CommonData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServerCommonData
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
                {
                    
                    var dish = JsonConvert.DeserializeObject<Dish>(data);   
                    _dbContext.Dishes.Add(dish);
                    await _dbContext.SaveChangesAsync();

                    //После добалвения блюда в БД получается его ID
                    if (dish.DishProducts != null && dish.DishProducts.Any())
                    {
                        foreach (var dishProduct in dish.DishProducts)
                        {
                            var productExists = await _dbContext.Products.AnyAsync(p => p.Id == dishProduct.ProductId);
                            if (!productExists)
                                return new Response { Success = false, Error = $"Product with ID {dishProduct.ProductId} not found." };

                            dishProduct.DishId = dish.Id;
                            _dbContext.DishProducts.Add(dishProduct);
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                     
                    if (dish.DishTags != null && dish.DishTags.Any())
                    {
                        foreach (var dishTag in dish.DishTags)
                        {
                            var tagExists = await _dbContext.Tags.AnyAsync(t => t.Id == dishTag.TagId);
                            if (!tagExists)
                                return new Response { Success = false, Error = $"Tag with ID {dishTag.TagId} not found." };

                            dishTag.DishId = dish.Id;
                            _dbContext.DishTags.Add(dishTag);
                        }
                        await _dbContext.SaveChangesAsync();
                    }

                    return new Response { Success = true };
                }
                case "Tag":
                {
                    var tag = JsonConvert.DeserializeObject<Tag>(data);
                    _dbContext.Tags.Add(tag);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                case "Product":
                {
                    var product = JsonConvert.DeserializeObject<Product>(data);
                    _dbContext.Products.Add(product);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                default:
                {
                    return new Response { Success = false, Error = "Unknown entity type" };
                }
            }
        }

        private async Task<Response> UpdateEntityAsync(string entityType, string data)
        {
            switch (entityType)
            {
                case "Dish":
                {
                    var updatedDish = JsonConvert.DeserializeObject<Dish>(data);

                    // 1. Находим существующее блюдо
                    var existingDish = await _dbContext.Dishes
                        .Include(d => d.DishProducts)
                        .Include(d => d.DishTags)
                        .FirstOrDefaultAsync(d => d.Id == updatedDish.Id);

                    if (existingDish == null)
                        return new Response { Success = false, Error = "Dish not found" };

                    // 2. Обновляем основные поля
                    _dbContext.Entry(existingDish).CurrentValues.SetValues(updatedDish);

                    // 3. Удаляем старые ингредиенты (DishProduct)
                    foreach (var existingProduct in existingDish.DishProducts.ToList())
                    {
                        _dbContext.DishProducts.Remove(existingProduct);
                    }

                    // 4. Добавляем новые ингредиенты
                    if (updatedDish.DishProducts != null)
                    {
                        foreach (var newProduct in updatedDish.DishProducts)
                        {
                            var productExists = await _dbContext.Products.AnyAsync(p => p.Id == newProduct.ProductId);
                            if (!productExists)
                                return new Response { Success = false, Error = $"Product with ID {newProduct.ProductId} not found." };

                            newProduct.DishId = updatedDish.Id;
                            _dbContext.DishProducts.Add(newProduct);
                        }
                    }

                    // 5. Удаляем старые теги (DishTag)
                    foreach (var existingTag in existingDish.DishTags.ToList())
                    {
                        _dbContext.DishTags.Remove(existingTag);
                    }

                    // 6. Добавляем новые теги
                    if (updatedDish.DishTags != null)
                    {
                        foreach (var newTag in updatedDish.DishTags)
                        {
                            var tagExists = await _dbContext.Tags.AnyAsync(t => t.Id == newTag.TagId);
                            if (!tagExists)
                                return new Response { Success = false, Error = $"Tag with ID {newTag.TagId} not found." };

                            newTag.DishId = updatedDish.Id;
                            _dbContext.DishTags.Add(newTag);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                case "Tag":
                {
                    var tag = JsonConvert.DeserializeObject<Tag>(data);
                    _dbContext.Tags.Update(tag);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                case "Product":
                {
                    var product = JsonConvert.DeserializeObject<Product>(data);
                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                    return new Response { Success = true };
                }
                default:
                {
                    return new Response { Success = false, Error = "Unknown entity type" };
                }
            }
        }

        private async Task<Response> DeleteEntityAsync(string entityType, string data)
        {
            switch (entityType)
            {
                case "Dish":
                {
                    var extdish = JsonConvert.DeserializeObject<Dish>(data);
                    int dishId = extdish.Id;
                    var dish = await _dbContext.Dishes.FindAsync(dishId);
                    if (dish != null)
                    {
                        _dbContext.Dishes.Remove(dish);
                        await _dbContext.SaveChangesAsync();
                        return new Response { Success = true };
                    }
                    return new Response { Success = false, Error = "Dish not found" };
                }
                case "Tag":
                {
                    var tagId = JsonConvert.DeserializeObject<int>(data);
                    var tag = await _dbContext.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        _dbContext.Tags.Remove(tag);
                        await _dbContext.SaveChangesAsync();
                        return new Response { Success = true };
                    }
                    return new Response { Success = false, Error = "Tag not found" };
                }
                case "Product":
                {
                    var productId = JsonConvert.DeserializeObject<int>(data);
                    var product = await _dbContext.Products.FindAsync(productId);
                    if (product != null)
                    {
                        _dbContext.Products.Remove(product);
                        await _dbContext.SaveChangesAsync();
                        return new Response { Success = true };
                    }
                    return new Response { Success = false, Error = "Product not found" };
                }
                default:
                {
                    return new Response { Success = false, Error = "Unknown entity type" };
                }
            }
        }
    }
}
