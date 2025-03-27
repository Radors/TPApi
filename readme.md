# TPApi

TPApi is the backend for *MatPerspektiv.se*

There are two endpoints:

* /food/search/basic  
* /food/search/embeddings  

The second endpoint compares embedding vectors, in order to search based on semantic meaning.  
This acts as a slower "backup" and finds some results which have not already been found by the basic search. 

The model used is the OpenAi Large Embedding Model 3, because of its competitive multi language support, including Swedish.

## Programming

[InputProcessor.cs](/TPApi/Food/Services/InputProcessor.cs ) 

[Program.cs](/TPApi/Program.cs)  


#### Main tech:  
• C# / .NET  
• ASP.NET Core Minimal API  
  
#### NuGet Packages:  
• Azure.Identity  
• Azure.Security.KeyVault.Secrets  
• Microsoft.EntityFrameworkCore  
• Microsoft.EntityFrameworkCore.Design  
• Microsoft.EntityFrameworkCore.SqlServer  
• Polly.Core  
• OpenAI  
