using SmartBOT.WebAPI.Core;



var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddSingleton<OpenAIChatService>();
builder.Services.AddSingleton<OpenAIEmbeddingsService>();
builder.Services.AddSingleton<AzureVectorSearchService>();
builder.Services.AddSingleton<SqLiteChatHistoryRepository>();
builder.Services.AddSingleton<TeslaHelpDeskService>();


var app = builder.Build();


// Configurar middlewares
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
