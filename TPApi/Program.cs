using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food.Services;
using TPApi.Food.Temporary;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TPDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
builder.Services.AddSingleton<EmbeddingsInMemory>();
builder.Services.AddSingleton<ProductsInMemory>();
// builder.Services.AddSingleton<UploadData>();

var app = builder.Build();

var embeddingsInMemory = app.Services.GetRequiredService<EmbeddingsInMemory>();
var productsInMemory = app.Services.GetRequiredService<ProductsInMemory>();
await embeddingsInMemory.LoadDataAsync();
await productsInMemory.LoadDataAsync();

// var uploadData = app.Services.GetRequiredService<UploadData>();
// await uploadData.EmbeddingsFromExcelToAzure();
// await uploadData.ProductsFromExcelToAzure();


app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapGet("/", () => "Here is a bit of text");
app.Run();