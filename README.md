## TPApi

TPApi is the backend for MatPerspektiv.se

There are two separate endpoints:

#### /food/search/basic  
• Very simple endpoint that does not actively engage any external dependencies nor any meaningful computational workload.  

#### /food/search/embeddings  
• Utilizes Embeddings from a state-of-the-art model to compare semantic meaning.  
• This endpoint acts as a slower "backup" and finds results which have not already been found by the basic search.  

--- 
#### Responsibilities of /food/search/embeddings:  
1. Accept request containing a search query.  
2. Send request to OpenAIs Large Embedding Model 3 to transform the query into a vector.  
   This vector is an array of 3072 floats.  
3. Compare this 1 newly returned vector with 2400 already available vectors (one for each food item in the database).  
   This comparison is done by computing the dot product between each pair.  
   Because the vectors returned by OpenAI are normalized, the dot product is therefore equivalent to cosine similarity.  
4. Select up to X top results within a maximum of Y similarity distance.  
5. Create FoodProductDTO instances for each of the top results with the actual nutritional values.  
6. Finally return the results. (Any redundant hits already found by the basic endpoint are then excluded by the frontend.)  
  
---   
#### Project Structure  

##### Minimal API endpoints are configured in Program.cs:  
• [Program.cs](/TPApi/Program.cs)  

##### InputProcessor is responsible for the work related to embeddings:  
• [InputProcessor.cs](/TPApi/Food/Services/InputProcessor.cs ) 
  
#### Central technologies:  
• C# / .NET  
• ASP.NET Core Minimal API  
  
#### NuGet Packages:  
• Azure.Identity  
• Azure.Security.KeyVault.Secrets  
• Microsoft.EntityFrameworkCore  
• Microsoft.EntityFrameworkCore.Design  
• Microsoft.EntityFrameworkCore.SqlServer  
• OpenAI  
• Polly.Core  
