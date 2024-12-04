using SmartBOT.WebAPI.Core;



var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddSingleton<IChatService, OpenAIChatService>();
builder.Services.AddSingleton<IEmbeddingsService, OpenAIEmbeddingsService>();
builder.Services.AddSingleton<IVectorDbSearchService, AzureVectorDbSearchService>();
builder.Services.AddSingleton<IChatHistoryRepository, SqLiteChatHistoryRepository>();
builder.Services.AddSingleton<IHelpDeskService, TeslaHelpDeskService>();


var app = builder.Build();


// Configurar middlewares
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
