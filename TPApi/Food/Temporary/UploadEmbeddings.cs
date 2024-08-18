using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Embeddings;
using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Temporary
{
    public class UploadEmbeddings
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UploadEmbeddings(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task ExcelToAzure()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();

                EmbeddingClient client = new("text-embedding-3-large", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<(int, string)> items = new();

                using (var stream = File.Open("C:\\Users\\Samuel\\source\\TPApi\\TPApi\\Food\\Temporary\\LivsmedelsDB.xlsx", FileMode.Open, FileAccess.Read))
                {
                    int counter1 = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                if (counter1 < 3)
                                {
                                    counter1++;
                                    continue;
                                }

                                items.Add(((int)reader.GetDouble(1), reader.GetString(0)));
                            }
                        } while (reader.NextResult());
                    }
                }

                string[] itemNames = items.Select(e => e.Item2).ToArray();

                EmbeddingCollection itemEmbeddings1 = await client.GenerateEmbeddingsAsync(new ArraySegment<string>(itemNames, 0, 2000));
                EmbeddingCollection itemEmbeddings2 = await client.GenerateEmbeddingsAsync(new ArraySegment<string>(itemNames, 2000, itemNames.Length-2000));

                ReadOnlyMemory<float>[] itemEmbeddings3 = itemEmbeddings1.ToArray().Concat(itemEmbeddings2.ToArray()).Select(e => e.Vector).ToArray();

                (int, float[])[] finalEmbeddings = new (int, float[])[itemEmbeddings3.Length];

                for (int i = 0; i < itemEmbeddings3.Length; i++)
                {
                    finalEmbeddings[i] = ( items[i].Item1, itemEmbeddings3[i].ToArray() );
                }

                foreach (var item in finalEmbeddings)
                {
                    Console.WriteLine(item.Item1);
                }
                Console.WriteLine("Total count: " + finalEmbeddings.Length);
            }
        }
    }
}
