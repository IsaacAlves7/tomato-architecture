using ProductService.Application.Mappings;
using ProductService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ── Camada "Pulp" (Application) ──────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<ProductMappingProfile>());

builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

// ── Camada "Flesh" (Infrastructure) ─────────────────────────────────────────
builder.Services.AddProductInfrastructure(builder.Configuration);

// ── Camada "Skin" (API) ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "🍅 ProductService API",
        Version = "v1",
        Description = "Microsserviço de Produtos — Tomato Architecture"
    });
    c.EnableAnnotations();
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductService v1");
    c.RoutePrefix = string.Empty;
});

app.UseSerilogRequestLogging();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
