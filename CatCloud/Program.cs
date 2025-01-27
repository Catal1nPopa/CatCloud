using Application.Configuration;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ApplicationInjection(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()  
              .AllowAnyMethod(); 
    });
});

try
{
    Log.Information("Aplicatia se porneste...");

    builder.Host.UseSerilog();  

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapScalarApiReference();
        app.MapOpenApi();
        Log.Information("Modul dezvoltare activat. OpenAPI si Scalar au fost mapate.");
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowSpecificOrigins");
    app.UseAuthorization();
    app.MapControllers();

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