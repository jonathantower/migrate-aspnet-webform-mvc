
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

app.UseAuthentication();
app.UseSystemWebAdapters();

app.MapDefaultControllerRoute();
app.MapRazorPages().RequireSystemWebAdapterSession();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"])
    .ShortCircuit()
    .Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();
