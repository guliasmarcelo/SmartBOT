using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartBOT;
using SmartBOT.WebAPI.Core;


var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os
builder.Services.AddControllers();
builder.Services.AddSingleton<OpenAIChatService>();
builder.Services.AddSingleton<OpenAIEmbeddingsService>();
builder.Services.AddSingleton<AzureAISearchService>();
builder.Services.AddSingleton<ChatHistoryRepository>();
builder.Services.AddSingleton<TeslaHelpDeskIntegrationService>();


var app = builder.Build();


// Configurar middlewares
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
