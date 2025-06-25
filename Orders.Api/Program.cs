using Microsoft.AspNetCore.Mvc;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.SignalR;
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


builder.Services.AddSignalR();

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();
app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");
app.UseCors();
app.Run();

public class OrderHub : Hub { }

[ApiController]
public class OrderController : ControllerBase
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IHubContext<OrderHub> hubContext, ILogger<OrderController> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost("/orders")]
    [Topic("pubsub", "orders")]
    public async Task<IActionResult> ReceiveOrder([FromBody] object payload)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOrder", payload);
        _logger.LogInformation("Order sent to clients via SignalR.");
        return Ok();
    }
}
