using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food;
using TPApi.Food.DBModels;
using TPApi.Food.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TPDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
builder.Services.AddSingleton<EmbeddingsInMemory>();
builder.Services.AddSingleton<ProductsInMemory>();
builder.Services.AddSingleton(e =>
{
    var vaultUrl = builder.Configuration["vaultUrl"]!;
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
    if (foodInputs.Length == 0) return Results.BadRequest();
    foreach (var input in foodInputs)
    {
        if (string.IsNullOrEmpty(input.Name)) return Results.BadRequest();
    }

    float[][] newEmbeddings = await inputProcessor.GetEmbeddingsAsync(foodInputs);

    if (embeddingsInMemory.TryGetEmbeddings() is FoodEmbedding[] storedEmbeddings &&
        productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        FoodAggregation[] aggregations = inputProcessor.GetAggregations(foodInputs, newEmbeddings, storedEmbeddings, storedProducts);
        return Results.Ok(aggregations);
    }
    return Results.StatusCode(503);
});
app.Run();