using Core.News;
using Core.NewsSearchs;
using Infraestructure;
using Infraestructure.EfCore;
using Infraestructure.NewsData;
using Infraestructure.NewsData.MemCache;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using WebApp.Components;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

var logUrl = builder.Configuration["Seq:Url"] ?? string.Empty;
var logApiKey = builder.Configuration["Seq:ApiKey"] ?? string.Empty;

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(x => x.AddOtlpExporter(a =>
{
    a.Endpoint = new Uri(logUrl);
    a.Protocol = OtlpExportProtocol.HttpProtobuf;
    a.Headers = "X-Seq-ApiKey=" + logApiKey;
}));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("database");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<NewsAzureSqlDatabase>();
builder.Services.AddSingleton<NewsMemCache>();
builder.Services.AddScoped<INewsRepository, NewsRepositoryProxy>();
builder.Services.AddScoped<INewsSearchRepository, NewsSearchAzureSqlDatabase>();
builder.Services.AddScoped<GetNews>();
builder.Services.AddScoped<RegisterNews>();

// OBSERVER PATTERN //
#pragma warning disable ASP0000
var registerNews = builder.Services.BuildServiceProvider().GetService<RegisterNews>();
#pragma warning restore ASP0000
var scrapNews = ScrapNews.GetInstance();
scrapNews.Subscribe(registerNews!);
//-----------------//

builder.Services.AddScoped<RegisterNewsSearch>();

builder.Services.AddBlazorBootstrap();

builder.Services.AddQuartz(q =>
{
    var scrapNewsJobKey = new JobKey("ScrapNewsJob");
    q.AddJob<ScrapNewsJob>(opts => opts.WithIdentity(scrapNewsJobKey).DisallowConcurrentExecution());

    q.AddTrigger(opts => opts
        .ForJob(scrapNewsJobKey)
        .WithIdentity("ScrapNewsJob-trigger")
        .StartNow()
        .WithCronSchedule("0 0 */3 ? * *")); // a cada 3 horas
        //.WithCronSchedule("0 0/1 * 1/1 * ? *")); // a cada 1 minuto
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
