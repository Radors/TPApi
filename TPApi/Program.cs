using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food;
using TPApi.Food.APIModels;
using TPApi.Food.DBModels;
using TPApi.Food.Services;

var builder = WebApplication.CreateBuilder(args);
string azureSqlConnectionString = builder.Configuration["AZURE_SQL_CONNECTIONSTRING"] ?? throw new InvalidOperationException("Azure SQL connectionstring not found");
builder.Services.AddDbContext<TPDbContext>(options => options.UseSqlServer(azureSqlConnectionString));
builder.Services.AddSingleton<EmbeddingsInMemory>();
builder.Services.AddSingleton<ProductsInMemory>();
string vaultUrl = builder.Configuration["vaultUrl"] ?? throw new InvalidOperationException("Vault URL not found");
builder.Services.AddSingleton(e =>
{
    return new SecretClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
});
builder.Services.AddSingleton<InputProcessor>();
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

app.MapPost("/food/processinput", async (FoodInput[] foodInputs) => {
    if (foodInputs.Length == 0)
    {
        return Results.BadRequest();
    }
    foreach (var input in foodInputs)
    {
        if (string.IsNullOrEmpty(input.Name)) return Results.BadRequest();
    }

    float[][] newEmbeddings = await inputProcessor.GetEmbeddingsAsync(foodInputs);

    if (embeddingsInMemory.TryGetEmbeddings() is FoodEmbedding[] storedEmbeddings &&
        productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        FoodProductDTO[] foodProductDTOs = inputProcessor.GetFoodProductDTOs(foodInputs, newEmbeddings, storedEmbeddings, storedProducts);
        return Results.Ok(foodProductDTOs);
    }
    return Results.StatusCode(503);
});
app.MapGet("/food/search", async (int frontendId, string query) =>
{
    if (string.IsNullOrEmpty(query))
    {
        return Results.BadRequest();
    }
    
    if (productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        var products = storedProducts.Where(e => e.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).OrderBy(e => e.Name.Length).Take(7);
        FoodProductDTO[] foodProductDTOs = products.Select(product => new FoodProductDTO(frontendId, product.Name, product)).ToArray();
        return Results.Ok(foodProductDTOs); //Returning an array of entire DTOs right away, in order to -not- need another request for the nutrition later
    }
    return Results.StatusCode(503);
});
app.Run();