using Innkeep.DI;
using MudBlazor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .WriteTo.Debug()
             .WriteTo.Trace()
             .MinimumLevel.Verbose()
             .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMudServices();

builder.WebHost.ConfigureKestrel(
	options =>
	{
		options.ListenAnyIP(1339);
		options.ListenAnyIP(1340, configure => configure.UseHttps());
	}
);

DependencyManager.InitializeClient(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();