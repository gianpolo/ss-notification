using Microsoft.AspNetCore.Mvc;
using Dapr;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:4000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddControllers().AddDapr();

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();
app.MapControllers();
app.UseCors();
app.Run();

[ApiController]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;

    public BasketController(ILogger<BasketController> logger)
    {
        _logger = logger;
    }

    [HttpPost("/baskets")]
    [Topic("pubsub", "baskets")]
    public async Task<IActionResult> ReceiveBasket([FromBody] object payload)
    {
        _logger.LogInformation("Basket received.");
        return Ok();
    }
}
