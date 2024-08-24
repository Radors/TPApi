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

var productsInMemory = app.Services.GetRequiredService<ProductsInMemory>();
var embeddingsInMemory = app.Services.GetRequiredService<EmbeddingsInMemory>();
await productsInMemory.LoadDataAsync(); // Implement logic inside the singleton to retry if connection takes a bunch of seconds (indicating the db was cold)
await embeddingsInMemory.LoadDataAsync(); // Since this is after above, db should be warm now, but still: Implement logic inside the singleton to retry if connection takes a bunch of seconds (indicating the db was cold)

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapPost("/food/processinput", async (FoodInput[] foodInputs) => {
    if (foodInputs is null || foodInputs.Length == 0) return Results.BadRequest();

    float[][] newEmbeddings = await InputProcessor.GetEmbeddingsAsync(foodInputs);

    if (embeddingsInMemory.TryGetEmbeddings() is FoodEmbedding[] storedEmbeddings &&
        productsInMemory.TryGetProducts() is FoodProduct[] storedProducts)
    {
        FoodAggregation[] aggregations = InputProcessor.GetAggregations(foodInputs, newEmbeddings, storedEmbeddings, storedProducts);
        return Results.Ok(aggregations);
    }
    return Results.StatusCode(503);
});
app.Run();