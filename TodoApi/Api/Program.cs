using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddPersistenceInfrastructure(_config);
//builder.Services.AddSharedInfrastructure(_config);
builder.Services.AddSharedInfrastructure();
builder.Services.AddSwaggerExtension();
builder.Services.AddApplicationLayer(_config);
builder.Services.AddControllers();
builder.Services.AddApiVersioningExtension();
builder.Services.AddSingleton(Log.Logger);

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
    options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "todoClient"));
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
