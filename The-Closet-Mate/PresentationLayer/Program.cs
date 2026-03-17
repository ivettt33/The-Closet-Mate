using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Database;
using DataAccessLayer.Repositories;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Interfaces.Repositories;
using BusinessLogicLayer.Interfaces.Services;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Core Services
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ClosetContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClosetMateDb")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ClosetContext>();

//  Dependency Injection
// Clothing
builder.Services.AddScoped<IClothingItemRepository, ClothingItemRepository>();
builder.Services.AddScoped<IClothingItemService, ClothingItemService>();

// Outfit
builder.Services.AddScoped<IOutfitRepository, OutfitRepository>();
builder.Services.AddScoped<IOutfitService, OutfitService>();
builder.Services.AddScoped<IOutfitQueryService, OutfitQueryService>();

// Scheduled Outfit
builder.Services.AddScoped<IScheduledOutfitRepository>(provider =>
    new ScheduledOutfitRepository(builder.Configuration.GetConnectionString("ClosetMateDb")));
builder.Services.AddScoped<IScheduledOutfitService, ScheduledOutfitService>();

// Auth
builder.Services.AddScoped<IUserService, UserService>();


// Build app
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    Console.WriteLine($"Requested URL: {context.Request.Path}");
    await next();
});

//  redirect
app.MapGet("/", context =>
{
    context.Response.Redirect("/Index");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();
