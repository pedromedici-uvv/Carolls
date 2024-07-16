using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitializeAsync(app);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
});


app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
 => HttpPolicyExtensions
     .HandleTransientHttpError()
     .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
     .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
