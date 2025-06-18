
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemWebAdapters()
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<string>("CartId");
    })
    .AddRemoteAppClient(options =>
    {
        options.RemoteAppUrl = new Uri(builder.Configuration["ProxyTo"]!);
        options.ApiKey = "760ea4f19eab4b5c909d3f61098e5f4c";
    })
.AddSessionClient()
.AddAuthenticationClient(true);

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WingtipToys"))
           .LogTo(Console.WriteLine));

builder.Services.AddHttpForwarder();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseRouting();

// Without this line, the authentication middleware will automatically be registered before 
// NOTE: Use[Routing/AUthentication/Authorization] will automatically be added and may not have to be explicit
// see https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
app.UseAuthentication();
//app.UseAuthorization();
app.UseSystemWebAdapters();

app.MapDefaultControllerRoute();
app.MapRazorPages().RequireSystemWebAdapterSession();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"])
    //, ForwarderRequestConfig.Empty, new StripDuplicateLocationHeaderTransformer())
    .ShortCircuit()
    .Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();
