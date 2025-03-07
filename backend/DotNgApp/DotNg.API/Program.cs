using DotNg.API.Configurations;
using DotNg.API.Configurations.Jwt;
using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

builder.Services.ConfigureJWTService();

builder.Services.ConfigureSwagger();

builder.Services.AddInfrastructure();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureGoogle(builder.Configuration);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    options.Events.OnRedirectToAccessDenied = context =>
//    {
//        context.Response.StatusCode = 403;
//        return Task.CompletedTask;
//    };
//    options.Events.OnRedirectToLogin = context =>
//    {
//        context.Response.StatusCode = 401;
//        return Task.CompletedTask;
//    };
//})
//.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
//{
//    options.ClientId = "182502122629-jendgi2jbor8unvh2a9jc0b17iqf16gl.apps.googleusercontent.com";
//    options.ClientSecret = "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";
//    options.CallbackPath = "/signin-google";
//    options.SaveTokens = true;
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//});

builder.Services.ConfigureServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseExceptionHandler();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
