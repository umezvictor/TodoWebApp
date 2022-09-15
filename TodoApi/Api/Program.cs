using App.Metrics;
using App.Metrics.AspNetCore;
using Serilog;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

System.Uri uri = new System.Uri("https://grafana.com/orgs/todoapp");

var metrics = AppMetrics.CreateDefaultBuilder()
    .OutputMetrics.AsPrometheusPlainText()
    .OutputMetrics.AsPrometheusProtobuf()
    .Report.ToHostedMetrics(options => {
        options.HostedMetrics.BaseUri = uri;
        options.HostedMetrics.ApiKey = "";
        options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
        options.HttpPolicy.FailuresBeforeBackoff = 5;
        options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
       
        options.FlushInterval = TimeSpan.FromSeconds(20);
    })
            .Build();

builder.Host.UseMetrics(options =>
{
    options.EndpointOptions = endpointsOptions =>
    {
        endpointsOptions.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
        endpointsOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusProtobufOutputFormatter>().First();
    };
});
builder.Host.UseMetricsEndpoints(options =>
{
    options.MetricsEndpointEnabled = true;
    options.MetricsTextEndpointEnabled = true;
    options.EnvironmentInfoEndpointEnabled = true;
});

builder.Host.UseMetricsWebTracking(options =>
{
    options.OAuth2TrackingEnabled = true;
    options.OAuth2TrackingEnabled = true;
    options.ApdexTrackingEnabled = true;    
   
});

var _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

builder.Services.AddControllers();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


//logging using Serilog
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddPersistenceInfrastructure(_config);
builder.Services.AddSharedInfrastructure();
builder.Services.AddSwaggerExtension();
builder.Services.AddApplicationLayer(_config);
builder.Services.AddControllers();
builder.Services.AddApiVersioningExtension();
builder.Services.AddSingleton(Log.Logger);


//monitoring

builder.Services.AddMvcCore().AddMetricsCore();



builder.Services.AddMetrics(metrics);
builder.Services.AddMetricsTrackingMiddleware();
builder.Services.AddMetricsEndpoints();



//Bearer:   default schema
builder.Services.AddAuthentication("Bearer")
       .AddJwtBearer("Bearer", options =>
       {
           options.Authority = "https://localhost:5001";
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateAudience = false
           };
       });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "a93d9ff5-fc41-401d-9007-6501553fbeaa"));
});



var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var env = app.Services.GetRequiredService<IWebHostEnvironment>();

var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;



if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors("MyPolicy");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerExtension(provider);
app.UseErrorHandlingMiddleware();

app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

//eyJrIjoiZDEwODYzZTEwYmJmYjRmNTNmYmRkOGEzZTcxZjU1MjZiNjdmOWQzYyIsIm4iOiJUb2RvQXBwS2V5IiwiaWQiOjcwOTQwMX0=

//todoapp2010.grafana.net

//eyJrIjoiOFI1c21DWkEzTlV6T1BkWW1pY3laRW0zMXY1NXZBMlUiLCJuIjoiVG9kb0FwcEtleSIsImlkIjoxfQ==


//curl - H "Authorization: Bearer eyJrIjoiOFI1c21DWkEzTlV6T1BkWW1pY3laRW0zMXY1NXZBMlUiLCJuIjoiVG9kb0FwcEtleSIsImlkIjoxfQ==" https://todoapp2010.grafana.net/api/dashboards/home