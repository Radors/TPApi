using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food.Services;
using TPApi.Food.Temporary;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TPDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
// builder.Services.AddSingleton<EmbeddingsInMemory>();
// builder.Services.AddSingleton<ProductsInMemory>();

builder.Services.AddSingleton<UploadEmbeddings>();

var app = builder.Build();
// app.Services.GetRequiredService<EmbeddingsInMemory>();
// app.Services.GetRequiredService<ProductsInMemory>();

var uploadEmbeddings = app.Services.GetRequiredService<UploadEmbeddings>();
await uploadEmbeddings.ExcelToAzure();

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapGet("/", () => "Here is a bit of text");
app.Run();