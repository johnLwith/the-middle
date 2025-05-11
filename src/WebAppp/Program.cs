using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using WebAppp.Data;
using System.Net.Http;
using WebAppp.Services;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


// Add HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<OllamaTextEmbeddingGeneration>();
builder.Services.AddScoped<INlpService, NlpService>();
builder.Services.AddScoped<ITranslateService, DeepseekTranslateService>();
builder.Services.AddScoped<IWordbookService, WordbookService>();

builder.Services.AddSingleton<Kernel>(x => {
    var configuration = x.GetRequiredService<IConfiguration>();
    var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0010
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: configuration["DeepSeek:ModelId"], // Optional name of the underlying model if the deployment name doesn't match the model name
        endpoint: new Uri(configuration["DeepSeek:Endpoint"]),
        apiKey: configuration["DeepSeek:ApiKey"]
    );
#pragma warning restore SKEXP0010

    return kernelBuilder.Build();
});

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "The Middle API",
        Version = "v1",
        Description = "API for managing episodes of The Middle"
    });
});

var app = builder.Build();

// Use CORS before other middleware
app.UseCors("ReactAppPolicy");

Console.WriteLine(app.Environment.EnvironmentName);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "The Middle API V1");
        c.RoutePrefix = "swagger";
    });
}


app.UseRouting();

// Configure API endpoints
app.MapControllers();

// Configure React app fallback only in production
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

app.Run();

