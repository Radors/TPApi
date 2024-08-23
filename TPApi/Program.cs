using Microsoft.EntityFrameworkCore;
using OpenAI.Embeddings;
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

app.MapPost("/food/processinput", async (FoodInput[] foodInputs, TPDbContext context) => {
    if (foodInputs.Length == 0) return Results.BadRequest();

    List<FoodAggregation> foodAggregations = new();
    foreach (var foodInput in foodInputs)
    {
        var newAggregation = new FoodAggregation(foodInput.FrontendId, foodInput.Name, foodInput.Weight);
        foodAggregations.Add(newAggregation);
    }

    EmbeddingClient client = new("text-embedding-3-large", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
    EmbeddingCollection newEmbeddings = await client.GenerateEmbeddingsAsync(foodAggregations.Select(e => e.Name).ToArray());
    float[][] arrayOfVectors = newEmbeddings.Select(e => e.Vector.ToArray()).ToArray();

    if (embeddingsInMemory.TryGetEmbeddings() is FoodEmbedding[] storedEmbeddings)
    {
        foreach (var newVector in arrayOfVectors)
        {
            // Continue here
        }
    }
    else return Results.StatusCode(503);

    return Results.Ok(); // Temporary
});
app.Run();