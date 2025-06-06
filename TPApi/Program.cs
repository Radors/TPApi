using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food;
using TPApi.Food.APIModels;
using TPApi.Food.DBModels;
using TPApi.Food.Services;

var builder = WebApplication.CreateBuilder(args);
string azureSqlConnectionString = builder.Configuration["AZURE_SQL_CONNECTIONSTRING"] 
                                  ?? throw new InvalidOperationException("Azure SQL connectionstring not found");

builder.Services.AddDbContext<TPDbContext>(options =>
    options.UseSqlServer(azureSqlConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 2,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        );
    }));

string vaultUrl = builder.Configuration["vaultUrl"] ?? throw new InvalidOperationException("Vault URL not found");
builder.Services.AddSingleton(e =>
{
    return new SecretClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
});

builder.Services.AddSingleton<InputProcessor>();
builder.Services.AddSingleton<ProductsInMemory>();
builder.Services.AddSingleton<EmbeddingsInMemory>();
var app = builder.Build();

var inputProcessor = app.Services.GetRequiredService<InputProcessor>();
var productsInMemory = app.Services.GetRequiredService<ProductsInMemory>();
var embeddingsInMemory = app.Services.GetRequiredService<EmbeddingsInMemory>();
await productsInMemory.LoadDataAsync();
await embeddingsInMemory.LoadDataAsync(); 

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapGet("/food/search/embeddings", async (string query, int frontendId) =>
{
    if (string.IsNullOrEmpty(query))
    {
        return Results.BadRequest();
    }

    float[] newEmbedding = await inputProcessor.GetEmbeddingAsync(query);

    if (embeddingsInMemory.TryGetEmbeddings() is FoodEmbedding[] storedEmbeddings &&
        productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        FoodProductDTO[] foodProductDTOs = inputProcessor.GetFoodProductDTOs(query, newEmbedding, storedEmbeddings, storedProducts, frontendId);
        return Results.Ok(foodProductDTOs);
    }
    return Results.StatusCode(503);
});
app.MapGet("/food/search/basic", (string query, int frontendId) =>
{
    if (string.IsNullOrEmpty(query))
    {
        return Results.BadRequest();
    }
    
    if (productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        var products = storedProducts.Where(e => e.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                                     .OrderBy(e => e.Name.Length)
                                     .Take(14);
        FoodProductDTO[] foodProductDTOs = products.Select(product => new FoodProductDTO(query, frontendId, product.Name, product))
                                                   .ToArray();
        return Results.Ok(foodProductDTOs);
    }
    return Results.StatusCode(503);
});
app.Run();