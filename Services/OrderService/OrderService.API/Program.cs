using OrderService.Application.Mappings;
using OrderService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ── Camada "Pulp" (Application) ──────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<OrderMappingProfile>());

builder.Services.AddAutoMapper(typeof(OrderMappingProfile));

// ── Camada "Flesh" (Infrastructure) ─────────────────────────────────────────
builder.Services.AddOrderInfrastructure(builder.Configuration);

// ── Camada "Skin" (API) ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "🍅 OrderService API",
        Version = "v1",
        Description = "Microsserviço de Pedidos — Tomato Architecture"
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService v1");
    c.RoutePrefix = string.Empty;
});

app.UseSerilogRequestLogging();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
