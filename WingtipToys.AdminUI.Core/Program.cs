using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

var path = Path.Combine(Path.GetTempPath(), "sharedkeys", "WingtipToys.Admin");
builder.Services.AddDataProtection()
    .SetApplicationName("WingtipToys.Admin")
    .PersistKeysToFileSystem(new DirectoryInfo(path));
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.Name = ".AspNet.SharedCookie";
});

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
builder.Services.AddHttpForwarder();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSystemWebAdapters();

app.MapDefaultControllerRoute();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"])
    .ShortCircuit()
    .Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();

