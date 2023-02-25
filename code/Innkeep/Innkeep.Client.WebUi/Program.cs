using Innkeep.Client.WebUi.Data;
using Innkeep.Core.Interfaces;
using Innkeep.Core.Interfaces.Pretix;
using Innkeep.Core.Interfaces.Repositories;
using Innkeep.Core.Interfaces.Services;
using Innkeep.Data.Repositories.FileOperations;
using Innkeep.Data.Repositories.Pretix;
using Innkeep.DI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddSingleton<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

builder.Services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>();

builder.Services.AddSingleton<IPretixRepository, PretixRepository>();
builder.Services.AddSingleton<IPretixService, PretixService>();

builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();

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