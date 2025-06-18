
using WingtipToys.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemWebAdapters()
    //.AddJsonSessionSerializer(options =>
    //{
    //    options.RegisterKey<string>("CartId");
    //})
    .AddRemoteAppClient(options =>
    {
        options.RemoteAppUrl = new Uri(builder.Configuration["ProxyTo"]!);
        options.ApiKey = "760ea4f19eab4b5c909d3f61098e5f4c";
    })
//.AddSessionClient()
.AddAuthenticationClient(true);

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WingtipToys"))
           .LogTo(Console.WriteLine));

builder.Services.AddHttpForwarder();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
//app.UseAuthentication();
app.UseSystemWebAdapters();

app.MapDefaultControllerRoute();

var legacyUri = app.Configuration["ProxyTo"];
app.MapForwarder("/{folder:regex(Content|Scripts)}/{**catch-all}", legacyUri).WithOrder(int.MaxValue - 1).ShortCircuit();
app.MapForwarder("/{**catch-all}", legacyUri)
    //, ForwarderRequestConfig.Empty, new StripDuplicateLocationHeaderTransformer())
    .ShortCircuit()
    .Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();
