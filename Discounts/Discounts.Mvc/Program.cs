using CloudinaryDotNet;
using Discounts.Application;
using Discounts.Application.Interfaces;
using Discounts.Infrastracture;
using Discounts.Infrastracture.Images;
using Mapster;
using MapsterMapper;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Discounts.Application.Common.Interfaces.ICurrentUserService, Discounts.Infrastracture.Services.CurrentUserService>();

// Mapster config (scan assembly)
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddSingleton<Microsoft.AspNetCore.Authentication.ISystemClock, Microsoft.AspNetCore.Authentication.SystemClock>();

// Cloudinary
var cloudinarySection = builder.Configuration.GetSection("Cloudinary");
var cloudinary = new Cloudinary(new Account(
    cloudinarySection["CloudName"],
    cloudinarySection["ApiKey"],
    cloudinarySection["ApiSecret"]));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<IImageStorageService, CloudinaryImageService>();

// Configure authentication scheme for Identity
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
