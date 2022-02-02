using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            // Seed producs brands.
            string brandsText = File.ReadAllText("..\\Infrastructure\\Data\\SeedData\\brands.json");
            List<ProductBrand> brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsText);

            foreach (ProductBrand brand in brands)
            {
                context.ProductBrands.Add(brand);
            }

            await context.SaveChangesAsync();

            // Seed producs types.
            string typesText = File.ReadAllText("..\\Infrastructure\\Data\\SeedData\\types.json");
            List<ProductType> types = JsonSerializer.Deserialize<List<ProductType>>(typesText);

            foreach (ProductType type in types)
            {
                context.ProductTypes.Add(type);
            }

            await context.SaveChangesAsync();

            // Seed producs.
            string productsText = File.ReadAllText("..\\Infrastructure\\Data\\SeedData\\products.json");
            List<Product> products = JsonSerializer.Deserialize<List<Product>>(productsText);

            foreach (Product product in products)
            {
                context.Products.Add(product);
            }

            await context.SaveChangesAsync();

        }
        catch (Exception exception)
        {

            loggerFactory.CreateLogger<StoreContextSeed>().LogError(exception.Message);
        }
    }
}
