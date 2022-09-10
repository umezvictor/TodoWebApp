using IdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddIdentityServer()
              .AddInMemoryClients(Config.Clients)
              .AddInMemoryApiResources(Config.ApiResources)
              .AddInMemoryApiScopes(Config.ApiScopes)
              .AddInMemoryIdentityResources(Config.IdentityResources)
              .AddTestUsers(Config.TestUsers)
              .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseIdentityServer();
//app.MapGet("/", () => "Hello World!");
app.UseEndpoints(endpoints => 
    endpoints.MapDefaultControllerRoute());

app.Run();
