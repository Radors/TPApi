﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Chat;
using OpenAI.Embeddings;
using System.Reflection.Metadata.Ecma335;
using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Temporary
{
    public class ManageData
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ManageData(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void WeightsFromGPTToAzure()
        {
            /*
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();
                FoodProduct[] products = context.FoodProducts.ToArray();
                foreach (var product in products)
                {
                    List<int> estimates = new();
                    while (estimates.Count < 3)
                    {
                        ChatClient client = new(model: "gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
                        ChatCompletion estimation = client.CompleteChat($"Hur mycket är det rimligt att en person av genomsnittlig vikt konsumerar av matvaran skriven inuti citatet: \"{product.Name}\". Ditt svar ska vara den exakta vikten i antal gram. Formatet bör vara strikt numeriskt och inte innehålla något sammanhang eller förklaring.");
                        if (Int32.TryParse(estimation.Content[0].Text, out int parsedEstimation))
                        {
                            estimates.Add(parsedEstimation);
                        }
                    }
                    double averageEstimate = (estimates[0] + estimates[1] + estimates[2]) / 3;
                    int roundedAverage;
                    if (averageEstimate > 100)
                    {
                        roundedAverage = (int)Math.Round(averageEstimate / 50) * 50;
                    }
                    else roundedAverage = (int)Math.Round(averageEstimate / 10, 0, MidpointRounding.AwayFromZero) * 10;
                    product.Weight = roundedAverage;
                    context.SaveChanges();
                    Console.WriteLine(product.Id);
                }           
            }
            */
        }

        public void ProductsFromExcelToAzure()
        {
            /*
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<FoodProduct> items = new();

                using (var stream = File.Open("C:\\Users\\Samuel\\source\\TPApi\\TPApi\\Food\\Temporary\\LivsmedelsDB.xlsx", FileMode.Open, FileAccess.Read))
                {
                    int counter = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                if (counter < 3)
                                {
                                    counter++;
                                    continue;
                                }
                                FoodProduct item = new FoodProduct() {
                                    OldId = (int)reader.GetDouble(1),
                                    Name = reader.GetString(0),
                                    Jod = reader.IsDBNull(51) ? 0f : (float)reader.GetDouble(51),
                                    Jarn = (float)reader.GetDouble(52),
                                    Kalcium = (float)reader.GetDouble(53),
                                    Kalium = (float)reader.GetDouble(54),
                                    Magnesium = (float)reader.GetDouble(55),
                                    Selen = (float)reader.GetDouble(58),
                                    Zink = (float)reader.GetDouble(59),
                                    A = (float)reader.GetDouble(36),
                                    B1 = (float)reader.GetDouble(42),
                                    B2 = (float)reader.GetDouble(43),
                                    B3 = (float)reader.GetDouble(45),
                                    B6 = (float)reader.GetDouble(46),
                                    B9 = (float)reader.GetDouble(47),
                                    B12 = (float)reader.GetDouble(48),
                                    C = (float)reader.GetDouble(49),
                                    D = (float)reader.GetDouble(39),
                                    E = (float)reader.GetDouble(40)
                                };
                                items.Add(item);
                            }
                        } while (reader.NextResult());
                    }
                }

                foreach (var item in items)
                {
                    context.FoodProducts.Add(item);
                }
                context.SaveChanges();
            }
            */
        }
        
        public void EmbeddingsFromGPTToAzure()
        {
            /*
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();

                EmbeddingClient client = new("text-embedding-3-large", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<(int, string)> items = new();

                using (var stream = File.Open("C:\\Users\\Samuel\\source\\TPApi\\TPApi\\Food\\Temporary\\LivsmedelsDB.xlsx", FileMode.Open, FileAccess.Read))
                {
                    int counter = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                if (counter < 3)
                                {
                                    counter++;
                                    continue;
                                }

                                items.Add(((int)reader.GetDouble(1), reader.GetString(0)));
                            }
                        } while (reader.NextResult());
                    }
                }

                string[] itemNames = items.Select(e => e.Item2).ToArray();

                EmbeddingCollection itemEmbeddings1 = client.GenerateEmbeddings(new ArraySegment<string>(itemNames, 0, 2000));
                EmbeddingCollection itemEmbeddings2 = client.GenerateEmbeddings(new ArraySegment<string>(itemNames, 2000, itemNames.Length-2000));

                float[][] itemEmbeddings3 = itemEmbeddings1.Concat(itemEmbeddings2).Select(e => e.Vector.ToArray()).ToArray();

                FoodEmbedding[] finalEmbeddings = new FoodEmbedding[itemEmbeddings3.Length];

                for (int i = 0; i < itemEmbeddings3.Length; i++)
                {
                    finalEmbeddings[i] = new FoodEmbedding() { OldId = items[i].Item1, Vector = itemEmbeddings3[i] };
                }

                
                foreach (var item in finalEmbeddings)
                {
                    context.FoodEmbeddings.Add(item);
                }
                context.SaveChanges();
            }
            */
        }
    }
}
