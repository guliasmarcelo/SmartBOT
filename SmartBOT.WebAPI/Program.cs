using SmartBOT.WebAPI.Core;



var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os
builder.Services.AddControllers();
builder.Services.AddSingleton<IChatService, OpenAIChatService>();
builder.Services.AddSingleton<IEmbeddingsService, OpenAIEmbeddingsService>();
builder.Services.AddSingleton<IVectorDbSearchService, AzureAISearchService>();
builder.Services.AddSingleton<IChatHistoryRepository, SqLiteChatHistoryRepository>();
builder.Services.AddSingleton<IHelpDeskService, TeslaHelpDeskService>();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); // Interface gr�fica do Swagger
app.UseRouting();

// Configurar middlewares
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
