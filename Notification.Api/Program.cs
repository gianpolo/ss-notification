using Microsoft.AspNetCore.Mvc;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SS.Services.Notification.Api.Events;

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
app.MapHub<NotificationHub>("/hubs/notifications");
app.UseCors();
app.Run();

public class NotificationHub : Hub { }

[ApiController]
public class NotificationController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(IHubContext<NotificationHub> hubContext, ILogger<NotificationController> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost("/notifications")]
    [Topic("pubsub", "notifications")]
    public async Task<IActionResult> ReceiveNotification([FromBody] object payload)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
        _logger.LogInformation("Notification sent to clients via SignalR.");
        return Ok();
    }

    [HttpPost("/events/basket-item-added")]
    [Topic("pubsub", "BasketItemAddedIntegrationEvent")]
    public async Task<IActionResult> OnBasketItemAdded([FromBody] BasketItemAddedIntegrationEvent @event)
    {
        await _hubContext.Clients.All.SendAsync("BasketItemAdded", @event);
        _logger.LogInformation("Basket item added event forwarded to SignalR clients.");
        return Ok();
    }
}
