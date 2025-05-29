using System;
using Application.Configuration;
using Application.Configuration.ExceptionConfig;
using CatCloud.ChatHub;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using CatCloud.NotificationHub;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 6L * 1024 * 1024 * 1024; // 6 GB
    });

    builder.Services.AddSignalR();
    builder.Services.ApplicationInjection(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins("https://localhost:3001", "http://localhost:3001", "http://localhost:4173", "http://localhost:80", "https://catstorage.cloud")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithExposedHeaders("Content-Disposition");
        });
    });

    Log.Information("Aplicatia se porneste...");

    builder.Host.UseSerilog();

    builder.Services.AddControllers();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

    builder.Services.AddAuthorization();

    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    if (app.Environment.IsDevelopment())
    {
        app.MapScalarApiReference();
        app.MapOpenApi();
        Log.Information("Modul dezvoltare activat. OpenAPI si Scalar au fost mapate.");
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowSpecificOrigins");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    
    app.MapHub<ChatHub>("/chatHub");
    app.MapHub<NotificationHub>("/notificationHub");

    Log.Information("Aplicatia a pornit cu succes si ruleaza...");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicatia s-a oprit in mod neasteptat.");
}
finally
{
    Log.CloseAndFlush();
}