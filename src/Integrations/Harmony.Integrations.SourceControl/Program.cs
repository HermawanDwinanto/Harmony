using Harmony.Application.Configurations;
using Harmony.Application.Extensions;
using Harmony.Application.Features.SourceControl.Commands.CreateBranch;
using Harmony.Application.SourceControl.Mappings;
using Harmony.Automations.Extensions;
using Harmony.Integrations.SourceControl.Extensions;
using Harmony.Integrations.SourceControl.HealthChecks;
using Harmony.Integrations.SourceControl.Services;
using Harmony.Logging;
using Harmony.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.AddRetryPolicies();
builder.Services.AddSwaggerGen();

// mongodb
builder.Services.AddMongoDbRepositories();
builder.Services.AddMongoDb(builder.Configuration);

// messaging
builder.Services.Configure<BrokerConfiguration>(builder.Configuration.GetSection("BrokerConfiguration"));
builder.Services.AddMessaging(builder.Configuration);

//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(typeof(SourceControlProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBranchCommand).Assembly));

// Add services to the container.
builder.Services.AddControllersWithViews();

// gRPC services
builder.Services.AddGrpc();

builder.Services.AddScoped<MongoDbHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<MongoDbHealthCheck>("mongodb", tags: ["ready"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => true
});

app.UseAuthorization();

app.MapGrpcService<SourceControlService>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.ConfigureSwagger();

app.Run();
