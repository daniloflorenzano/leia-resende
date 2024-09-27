using Core.News;
using Core.NewsSearchs;
using Infraestructure;
using Infraestructure.Azure;
using Infraestructure.NewsData;
using WebApp.Components;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var azureCosmosDbConfig = new AzureCosmosDbConfig();
builder.Configuration.GetSection("AzureCosmosDB").Bind(azureCosmosDbConfig);
builder.Services.AddSingleton(azureCosmosDbConfig);

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddScoped<NewsAzureCosmosDb>();
builder.Services.AddSingleton<NewsMemCache>();
builder.Services.AddScoped<INewsRepository, NewsRepositoryProxy>();
builder.Services.AddScoped<INewsSearchRepository, NewsSearchAzureCosmosDb>();

builder.Services.AddScoped<GetNews>();
builder.Services.AddScoped<RegisterNews>();
var registerNews = builder.Services.BuildServiceProvider().GetService<RegisterNews>();
var scrapNews = ScrapNews.GetInstance();
scrapNews.Subscribe(registerNews!);

builder.Services.AddScoped<RegisterNewsSearch>();

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
