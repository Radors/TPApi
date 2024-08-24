using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food;
using TPApi.Food.DBModels;
using TPApi.Food.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TPDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
builder.Services.AddSingleton<EmbeddingsInMemory>();
builder.Services.AddSingleton<ProductsInMemory>();
var app = builder.Build();

var embeddingsInMemory = app.Services.GetRequiredService<EmbeddingsInMemory>();
var productsInMemory = app.Services.GetRequiredService<ProductsInMemory>();
await embeddingsInMemory.LoadDataAsync();
await productsInMemory.LoadDataAsync();

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapPost("/food/processinput", async (FoodInput[] foodInputs) => {
    if (foodInputs.Length == 0) return Results.BadRequest();

    float[][] newEmbeddings = await InputProcessor.GetEmbeddingsAsync(foodInputs);

    if (embeddingsInMemory.TryGetEmbeddings() is FoodEmbedding[] storedEmbeddings &&
        productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        FoodAggregation[] aggregations = InputProcessor.GetAggregations(foodInputs, newEmbeddings, storedEmbeddings, storedProducts);
        return Results.Ok(aggregations);
    }
    else return Results.StatusCode(503);
});
app.Run();